import logging
import json
import subprocess
import sys
from datetime import datetime, timezone
import uuid

logger = logging.getLogger(__name__)


def _run_playwright_subprocess(source: str, url: str, max_count: int) -> list:
    """
    Run Playwright in a separate Python subprocess to avoid Windows asyncio conflicts.
    """
    script = f"""
import sys
import json
from datetime import datetime, timezone
import uuid
from playwright.sync_api import sync_playwright, TimeoutError as PlaywrightTimeoutError

def scrape():
    reviews = []
    debug_info = {{"found_elements": 0, "stage": "start"}}
    
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        context = browser.new_context(
            user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        )
        page = context.new_page()
        
        try:
            debug_info["stage"] = "navigating"
            page.goto("{url}", wait_until="domcontentloaded", timeout=60000)
            page.wait_for_timeout(5000)  # Wait 5 seconds for JS to render
            debug_info["stage"] = "page_loaded"
            
            # Cookie consent
            try:
                buttons = page.query_selector_all("button:has-text('Accept'), button#onetrust-accept-btn-handler")
                if buttons:
                    buttons[0].click()
                    page.wait_for_timeout(1000)
                    debug_info["cookie_handled"] = True
            except Exception as e:
                debug_info["cookie_error"] = str(e)
            
            if "{source}".lower() == "tripadvisor":
                debug_info["stage"] = "finding_reviews"
                
                # Try multiple selectors
                elements = page.query_selector_all("div[data-automation='reviewCard']")
                debug_info["selector1_count"] = len(elements)
                
                if not elements:
                    elements = page.query_selector_all(".review-container")
                    debug_info["selector2_count"] = len(elements)
                
                if not elements:
                    elements = page.query_selector_all("div[data-test-target='HR_CC_CARD']")
                    debug_info["selector3_count"] = len(elements)
                
                debug_info["found_elements"] = len(elements)
                debug_info["stage"] = "parsing_reviews"
                
                for i, el in enumerate(elements[:{max_count}]):
                    try:
                        # Try different content selectors
                        content_el = el.query_selector("span[data-automation='reviewText']")
                        if not content_el:
                            content_el = el.query_selector(".partial_entry")
                        if not content_el:
                            content_el = el.query_selector("q span")
                        if not content_el:
                            content_el = el.query_selector("div.fIrGe")
                        
                        content = content_el.inner_text() if content_el else ""
                        
                        if not content.strip():
                            debug_info[f"review_{{i}}_no_content"] = True
                            continue
                        
                        # Author
                        author_el = el.query_selector("a.ui_header_link")
                        if not author_el:
                            author_el = el.query_selector("div.info_text div:first-child")
                        author = author_el.inner_text() if author_el else "Anonim"
                        
                        ext_id = el.get_attribute("data-reviewid") or f"ta_{{uuid.uuid4().hex[:8]}}"
                        
                        reviews.append({{
                            "external_id": ext_id,
                            "content": content.strip(),
                            "author_name": author.strip(),
                            "reviewed_at": datetime.now(timezone.utc).isoformat()
                        }})
                        debug_info[f"review_{{i}}_success"] = True
                    except Exception as e:
                        debug_info[f"review_{{i}}_error"] = str(e)
            
            debug_info["final_count"] = len(reviews)
            debug_info["stage"] = "complete"
        
        except PlaywrightTimeoutError as e:
            debug_info["error"] = f"Timeout: {{str(e)}}"
        except Exception as e:
            debug_info["error"] = str(e)
            debug_info["error_type"] = type(e).__name__
        finally:
            browser.close()
    
    # Log debug info to stderr for troubleshooting
    print(json.dumps(debug_info), file=sys.stderr)
    print(json.dumps(reviews))

if __name__ == "__main__":
    scrape()
"""
    
    try:
        result = subprocess.run(
            [sys.executable, "-c", script],
            capture_output=True,
            text=True,
            timeout=90,
            check=True
        )
        
        if result.stderr:
            try:
                debug_info = json.loads(result.stderr)
                logger.info(f"Scraper debug info: {debug_info}")
            except:
                logger.warning(f"Playwright subprocess stderr: {result.stderr}")
        
        reviews = json.loads(result.stdout)
        logger.info(f"Subprocess returned {len(reviews)} reviews")
        return reviews
        
    except subprocess.TimeoutExpired:
        logger.error("Playwright subprocess timed out after 90s")
        raise Exception("Scraping timeout - page took too long to load")
    except subprocess.CalledProcessError as e:
        logger.error(f"Playwright subprocess failed: {e.stderr}")
        raise Exception(f"Scraping failed: {e.stderr}")
    except json.JSONDecodeError as e:
        logger.error(f"Failed to parse subprocess output: {result.stdout}")
        raise Exception("Invalid scraping response")
    except Exception as e:
        logger.error(f"Subprocess execution failed: {e}")
        raise


async def scrape_reviews(url: str, source: str, sort_by: str, max_count: int) -> list:
    """
    Entry point for scraping reviews.
    Runs Playwright in a completely separate subprocess to avoid asyncio conflicts.
    """
    logger.info(f"Starting scrape for {source} from {url}")
    
    import asyncio
    loop = asyncio.get_event_loop()
    
    # Run the subprocess in executor to not block FastAPI
    from concurrent.futures import ThreadPoolExecutor
    executor = ThreadPoolExecutor(max_workers=1)
    
    try:
        reviews = await loop.run_in_executor(
            executor, 
            _run_playwright_subprocess, 
            source.lower(), 
            url, 
            max_count
        )
        logger.info(f"Scraping completed: {len(reviews)} reviews")
        return reviews
    finally:
        executor.shutdown(wait=False)