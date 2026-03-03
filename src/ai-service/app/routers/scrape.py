from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
from typing import List, Optional
from datetime import datetime
import logging
from app.services.review_scraper import scrape_reviews

logger = logging.getLogger(__name__)
router = APIRouter()

class ScrapeRequest(BaseModel):
    url: str
    source: str
    sort_by: str = "newest"
    max_count: int = 20

class ScrapeResponseItem(BaseModel):
    external_id: str
    content: str
    author_name: Optional[str] = None
    reviewed_at: Optional[datetime] = None

@router.post("/scrape", response_model=List[ScrapeResponseItem])
async def scrape_endpoint(request: ScrapeRequest):
    """
    Endpoint pentru scraping recenzii din surse externe.
    """
    try:
        logger.info(f"Received scrape request: source={request.source}, url={request.url}, max_count={request.max_count}")
        
        reviews = await scrape_reviews(
            request.url, 
            request.source, 
            request.sort_by, 
            request.max_count
        )
        
        logger.info(f"Scraping completed successfully: {len(reviews)} reviews extracted")
        return reviews
        
    except Exception as e:
        logger.error(f"Scraping failed: {str(e)}", exc_info=True)
        raise HTTPException(
            status_code=500, 
            detail=f"Scraping error for {request.source}: {str(e)}"
        )