"""
Router FastAPI pentru endpoint-ul de analiză a sentimentului.
"""

from fastapi import APIRouter, HTTPException

from app.models.schemas import SentimentRequest, SentimentResponse
from app.services.sentiment_analyzer import analyze_sentiment

router = APIRouter()


@router.post(
    "/analyze",
    response_model=SentimentResponse,
    summary="Analizează sentimentul unei recenzii",
    description="Primește textul recenziei și returnează eticheta de sentiment și scorul de încredere.",
)
async def analyze(request: SentimentRequest) -> SentimentResponse:
    """
    Endpoint principal de analiză a sentimentului.

    - **text**: Textul recenziei (1-5000 caractere)
    - **Returns**: label (Positive/Negative/Neutral) + score (0.0-1.0)
    """
    try:
        result = analyze_sentiment(request.text)
        return SentimentResponse(label=result["label"], score=result["score"])
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"Eroare la analiza sentimentului: {str(e)}",
        ) from e
