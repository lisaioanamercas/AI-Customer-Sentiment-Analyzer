"""
Serviciu de analiză a sentimentului folosind Hugging Face Transformers.
Încarcă modelul DistilBERT o singură dată și oferă predicții rapide.
"""

import logging
from functools import lru_cache

from transformers import pipeline  # type: ignore

logger = logging.getLogger(__name__)

# Maparea etichetelor modelului HF la etichetele AISA
LABEL_MAP = {
    "POSITIVE": "Positive",
    "NEGATIVE": "Negative",
    "NEUTRAL": "Neutral",
    # Modelul distilbert-base-uncased-finetuned-sst-2-english are doar POSITIVE/NEGATIVE
    # Vom trata scoruri joase ca Neutral
}


@lru_cache(maxsize=1)
def get_sentiment_pipeline():
    """
    Încarcă pipeline-ul de sentiment analysis o singură dată (singleton via cache).
    Folosește distilbert-base-uncased-finetuned-sst-2-english.
    """
    logger.info("Încărcare model de sentiment analysis...")
    sentiment_pipeline = pipeline(
        "sentiment-analysis",
        model="distilbert-base-uncased-finetuned-sst-2-english",
    )
    logger.info("Model încărcat cu succes!")
    return sentiment_pipeline


def analyze_sentiment(text: str) -> dict:
    """
    Analizează sentimentul textului furnizat.

    Args:
        text: Textul recenziei de analizat.

    Returns:
        Dict cu 'label' (Positive/Negative/Neutral) și 'score' (0.0-1.0).
    """
    sentiment_pipeline = get_sentiment_pipeline()

    # Trunchierea la 512 tokeni (limita modelului)
    result = sentiment_pipeline(text[:512])[0]

    raw_label = result["label"]
    score = round(result["score"], 4)

    # Mapare label
    label = LABEL_MAP.get(raw_label, "Neutral")

    # Dacă scorul e foarte scăzut, considerăm Neutral
    if score < 0.6:
        label = "Neutral"

    logger.info(
        "Analiză completată: label=%s, score=%.4f (raw: %s)",
        label,
        score,
        raw_label,
    )

    return {"label": label, "score": score}
