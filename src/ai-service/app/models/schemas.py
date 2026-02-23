"""
Modele Pydantic pentru request/response AI Sentiment Analysis.
"""

from pydantic import BaseModel, Field


class SentimentRequest(BaseModel):
    """Request body pentru analiza de sentiment."""

    text: str = Field(
        ...,
        min_length=1,
        max_length=5000,
        description="Textul recenziei de analizat",
        examples=["Mâncarea a fost excelentă, personal foarte amabil!"],
    )


class SentimentResponse(BaseModel):
    """Response body cu rezultatul analizei."""

    label: str = Field(
        ...,
        description="Eticheta sentimentului: Positive, Negative, sau Neutral",
        examples=["Positive"],
    )
    score: float = Field(
        ...,
        ge=0.0,
        le=1.0,
        description="Scorul de încredere al modelului (0.0 - 1.0)",
        examples=[0.95],
    )


class HealthResponse(BaseModel):
    """Response body pentru health check."""

    status: str
    service: str
