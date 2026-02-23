"""
AISA AI Sentiment Analysis Microservice
FastAPI application for analyzing customer review sentiment using DistilBERT.
"""

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from app.routers import analyze

app = FastAPI(
    title="AISA AI Sentiment Service",
    description="Microserviciu de analiză a sentimentului folosind DistilBERT",
    version="1.0.0",
)

# CORS — permite apeluri de la backend-ul .NET
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Înregistrăm router-ul de analiză
app.include_router(analyze.router, prefix="/api", tags=["Sentiment Analysis"])


@app.get("/health")
async def health_check():
    """Health check endpoint."""
    return {"status": "healthy", "service": "ai-sentiment"}
