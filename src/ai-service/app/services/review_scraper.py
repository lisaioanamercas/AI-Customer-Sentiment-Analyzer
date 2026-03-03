import logging
import time
from datetime import datetime, timezone
import uuid
from playwright.sync_api import sync_playwright

logger = logging.getLogger(__name__)

def scrape_tripadvisor(url: str, max_count: int) -> list:
    """
    Scrape real reviews from TripAdvisor using Playwright in synchronous mode.
    """
    reviews = []
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        context = browser.new_context(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        )
        page = context.new_page()
        
        try:
            logger.info(f"Navigating to TripAdvisor URL: {url}")
            page.goto(url, wait_until="networkidle", timeout=60000)
            
            # 1. Handle Cookie Consent
            try:
                accept_btns = page.query_selector_all("button:has-text('Accept'), button#onetrust-accept-btn-handler")
                if accept_btns:
                    accept_btns[0].click()
                    time.sleep(1)
            except Exception: pass

            # 2. Expand all "More" buttons
            try:
                more_btns = page.query_selector_all("span:has-text('More'), span.exP7L")
                for btn in more_btns:
                    btn.click()
                    time.sleep(0.5)
            except Exception: pass

            # 3. Find review cards
            review_elements = page.query_selector_all("div[data-automation='reviewCard']")
            if not review_elements:
                review_elements = page.query_selector_all(".review-container")

            for el in review_elements[:max_count]:
                try:
                    content_el = el.query_selector("span[data-automation='reviewText'], .partial_entry")
                    content = content_el.inner_text() if content_el else ""
                    
                    author_el = el.query_selector(".ui_header_link, a[href*='/Profile/']")
                    author = author_el.inner_text() if author_el else "Anonim"
                    
                    ext_id_attr = el.get_attribute("data-reviewid") or f"ta_{uuid.uuid4().hex[:8]}"
                    
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
            browser.close()
            
    return reviews


def scrape_google(url: str, max_count: int) -> list:
    """
    Scrape reviews from Google Search / Maps using Playwright in synchronous mode.
    """
    reviews = []
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        context = browser.new_context(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        )
        page = context.new_page()
        
        try:
            logger.info(f"Navigating to Google URL: {url}")
            page.goto(url, wait_until="networkidle", timeout=60000)
            
            # 1. Handle Google Cookie Consent
            try:
                accept_btns = page.query_selector_all("button:has-text('Accept all'), button:has-text('Accept All'), button:has-text('I agree')")
                if accept_btns:
                    accept_btns[0].click()
                    time.sleep(1)
            except Exception: pass

            # 2. Wait for rendering
            time.sleep(3)
            
            review_elements = page.query_selector_all(".jfti1, .gws-localreviews__google-review, .mG61Hc, div[jscontroller='f6979e']")
            
            if not review_elements:
                # Try Maps selectors
                review_elements = page.query_selector_all("div.jsti1, .wi7C8e")

            for el in review_elements[:max_count]:
                try:
                    # Content
                    content_el = el.query_selector(".wi7C8e, .K70WS, span.wi7C8e, .review-full-text")
                    content = content_el.inner_text() if content_el else ""
                    
                    # Author
                    author_el = el.query_selector(".d4r55, .TSZ60d, .X8779b")
                    author = author_el.inner_text() if author_el else "Anonim"
                    
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
            browser.close()
            
    return reviews


def scrape_reviews(url: str, source: str, sort_by: str, max_count: int) -> list:
    """
    Entry point for scraping reviews.
    """
    logger.info(f"Starting scrape for {source} from {url}")
    
    if source.lower() == "tripadvisor":
        return scrape_tripadvisor(url, max_count)
    
    if source.lower() == "google":
        return scrape_google(url, max_count)
        
    return []
