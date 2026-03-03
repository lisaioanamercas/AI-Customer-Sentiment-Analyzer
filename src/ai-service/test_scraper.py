"""
Test what content is actually in the review cards
"""
from playwright.sync_api import sync_playwright
import time

url = "https://www.tripadvisor.com/Restaurant_Review-g304060-d4993091-Reviews-Toujours-Iasi_Iasi_County_Northeast_Romania.html"

print("Starting Playwright with better bot avoidance...")
with sync_playwright() as p:
    browser = p.chromium.launch(
        headless=False,
        args=[
            '--disable-blink-features=AutomationControlled',
            '--disable-features=IsolateOrigins,site-per-process'
        ]
    )
    
    context = browser.new_context(
        user_agent="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
        viewport={'width': 1920, 'height': 1080},
        locale='en-US',
        timezone_id='Europe/Bucharest'
    )
    
    # Hide webdriver property
    page = context.new_page()
    page.add_init_script("""
        Object.defineProperty(navigator, 'webdriver', {
            get: () => undefined
        });
    """)
    
    print(f"Navigating to: {url}")
    page.goto(url, wait_until="domcontentloaded")
    print("Page loaded, waiting 8 seconds...")
    time.sleep(8)
    
    # Check for cookie consent
    try:
        cookie_btn = page.query_selector("button#onetrust-accept-btn-handler, button:has-text('Accept')")
        if cookie_btn:
            print("Clicking cookie consent...")
            cookie_btn.click()
            time.sleep(2)
    except:
        pass
    
    # Find review cards
    review_cards = page.query_selector_all("div[data-automation='reviewCard']")
    print(f"\nFound {len(review_cards)} review cards")
    
    # Check first 3 cards for content
    for i, card in enumerate(review_cards[:3]):
        print(f"\n=== Review Card {i+1} ===")
        
        # Try to find review text
        text_el = card.query_selector("span[data-automation='reviewText']")
        if text_el:
            text = text_el.inner_text()
            print(f"Text: {text[:100]}...")
        else:
            print("❌ No reviewText found")
        
        # Try alternative selector
        text_el2 = card.query_selector("q span")
        if text_el2:
            text2 = text_el2.inner_text()
            print(f"Alt Text: {text2[:100]}...")
        else:
            print("❌ No alt text found")
        
        # Get all text content from card
        all_text = card.inner_text()
        print(f"Card full text (first 150 chars): {all_text[:150]}...")
    
    # Check page content for CAPTCHA
    page_text = page.content()
    if "captcha" in page_text.lower():
        print("\n🚨 CAPTCHA DETECTED IN PAGE SOURCE")
    
    if "challenge" in page_text.lower():
        print("🚨 Challenge page detected")
    
    # Save full page HTML for inspection
    with open("page_source.html", "w", encoding="utf-8") as f:
        f.write(page_text)
    print("\n✅ Full page HTML saved to: page_source.html")
    
    page.screenshot(path="tripadvisor_debug2.png", full_page=True)
    print("✅ Full page screenshot saved to: tripadvisor_debug2.png")
    
    print("\nBrowser will close in 10 seconds...")
    time.sleep(10)
    
    browser.close()

print("\nTest complete! Check page_source.html to see what TripAdvisor served.")