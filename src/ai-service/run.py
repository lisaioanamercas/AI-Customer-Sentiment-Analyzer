"""
Custom uvicorn startup script for Windows compatibility with Playwright.
This sets the correct event loop policy BEFORE uvicorn creates its event loop.
"""
import sys
import asyncio

# CRITICAL: Set event loop policy for Windows BEFORE importing uvicorn
if sys.platform == "win32":
    asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy())

if __name__ == "__main__":
    import uvicorn
    
    uvicorn.run(
        "app.main:app",
        host="0.0.0.0",
        port=8000,
        reload=True,
        log_level="info"
    )