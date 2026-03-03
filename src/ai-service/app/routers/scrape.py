from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
from typing import List, Optional
from datetime import datetime
from app.services.review_scraper import scrape_reviews

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
def scrape_endpoint(request: ScrapeRequest):
    """
    Endpoint pentru scraping recenzii din surse externe.
    Executat sincron de FastAPI (într-un thread izolat).
    """
    try:
        reviews = scrape_reviews(
            request.url, 
            request.source, 
            request.sort_by, 
            request.max_count
        )
        return reviews
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
