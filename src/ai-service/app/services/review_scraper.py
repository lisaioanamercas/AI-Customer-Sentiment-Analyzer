import logging
import asyncio
from datetime import datetime, timezone
import uuid
import re
from playwright.async_api import async_playwright

logger = logging.getLogger(__name__)

async def scrape_tripadvisor(url: str, max_count: int) -> list:
    """
    Scrape real reviews from TripAdvisor using Playwright.
    """
    reviews = []
    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        context = await browser.new_context(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        )
        page = await context.new_page()
        
        try:
            logger.info(f"Navigating to TripAdvisor URL: {url}")
            await page.goto(url, wait_until="networkidle", timeout=60000)
            
            # 1. Handle Cookie Consent
            try:
                accept_btns = await page.query_selector_all("button:has-text('Accept'), button#onetrust-accept-btn-handler")
                if accept_btns:
                    await accept_btns[0].click()
                    await asyncio.sleep(1)
            except Exception: pass

            # 2. Expand all "More" buttons
            try:
                more_btns = await page.query_selector_all("span:has-text('More'), span.exP7L")
                for btn in more_btns:
                    await btn.click()
                    await asyncio.sleep(0.5)
            except Exception: pass

            # 3. Find review cards
            review_elements = await page.query_selector_all("div[data-automation='reviewCard']")
            if not review_elements:
                review_elements = await page.query_selector_all(".review-container")

            for el in review_elements[:max_count]:
                try:
                    content_el = await el.query_selector("span[data-automation='reviewText'], .partial_entry")
                    content = await content_el.inner_text() if content_el else ""
                    
                    author_el = await el.query_selector(".ui_header_link, a[href*='/Profile/']")
                    author = await author_el.inner_text() if author_el else "Anonim"
                    
                    ext_id_attr = await el.get_attribute("data-reviewid") or f"ta_{uuid.uuid4().hex[:8]}"
                    
                    reviews.append({
                        "external_id": ext_id_attr,
                        "content": content,
                        "author_name": author,
                        "reviewed_at": datetime.now(timezone.utc).isoformat()
                    })
                except Exception as inner_e:
                    logger.error(f"Error parsing TripAdvisor review: {inner_e}")
                    
        except Exception as e:
            logger.error(f"TripAdvisor scraping failed: {e}")
        finally:
            await browser.close()
            
    return reviews

async def scrape_google(url: str, max_count: int) -> list:
    """
    Scrape reviews from Google Search / Maps using Playwright.
    """
    reviews = []
    async with async_playwright() as p:
        browser = await p.chromium.launch(headless=True)
        context = await browser.new_context(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        )
        page = await context.new_page()
        
        try:
            logger.info(f"Navigating to Google URL: {url}")
            await page.goto(url, wait_until="networkidle", timeout=60000)
            
            # 1. Handle Google Cookie Consent
            try:
                accept_btns = await page.query_selector_all("button:has-text('Accept all'), button:has-text('Accept All'), button:has-text('I agree')")
                if accept_btns:
                    await accept_btns[0].click()
                    await asyncio.sleep(1)
            except Exception: pass

            # 2. If it's a search page, we might need to click "More reviews" or "Reviews"
            # But the user provided link already has some review context.
            # Google Search review elements often use class "gws-localreviews__google-review"
            await asyncio.sleep(2) # Wait for JS rendering
            
            review_elements = await page.query_selector_all(".jfti1, .gws-localreviews__google-review, .mG61Hc, div[jscontroller='f6979e']")
            
            if not review_elements:
                # Try Maps selectors
                review_elements = await page.query_selector_all("div.jsti1, .wi7C8e")

            for el in review_elements[:max_count]:
                try:
                    # Content
                    content_el = await el.query_selector(".wi7C8e, .K70WS, span.wi7C8e, .review-full-text")
                    # In some views, it's inside a button or hidden
                    content = await content_el.inner_text() if content_el else ""
                    
                    # Author
                    author_el = await el.query_selector(".d4r55, .TSZ60d, .X8779b")
                    author = await author_el.inner_text() if author_el else "Anonim"
                    
                    # ID
                    ext_id = f"g_{uuid.uuid4().hex[:8]}"
                    
                    if content.strip():
                        reviews.append({
                            "external_id": ext_id,
                            "content": content,
                            "author_name": author,
                            "reviewed_at": datetime.now(timezone.utc).isoformat()
                        })
                except Exception as inner_e:
                    logger.debug(f"Error parsing Google review: {inner_e}")
                    
        except Exception as e:
            logger.error(f"Google scraping failed: {e}")
        finally:
            await browser.close()
            
    return reviews

async def scrape_reviews(url: str, source: str, sort_by: str, max_count: int) -> list:
    """
    Entry point for scraping reviews.
    """
    logger.info(f"Starting scrape for {source} from {url}")
    
    if source.lower() == "tripadvisor":
        return await scrape_tripadvisor(url, max_count)
    
    if source.lower() == "google":
        return await scrape_google(url, max_count)
        
    return []
