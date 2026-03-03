"""
Serviciu de analiză a sentimentului folosind Hugging Face Transformers.
Încarcă modelul DistilBERT o singură dată și oferă predicții rapide.
"""

import logging
from functools import lru_cache

from transformers import pipeline  # type: ignore

logger = logging.getLogger(__name__)

# Cardiff NLP XLM-RoBERTa can return labels in any casing
LABEL_MAP = {
    "positive": "Positive",
    "negative": "Negative",
    "neutral": "Neutral",
}


@lru_cache(maxsize=1)
def get_sentiment_pipeline():
    """
    Încarcă pipeline-ul de sentiment analysis o singură dată (singleton via cache).
    Folosește XLM-RoBERTa multilingual — suportă română, engleză și alte 8 limbi.
    Download ~500MB la prima rulare, după aceea e cached local.
    """
    logger.info("Încărcare model de sentiment analysis multilingual...")
    sentiment_pipeline = pipeline(
        "sentiment-analysis",
        model="cardiffnlp/twitter-xlm-roberta-base-sentiment",
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

    try:
        # top_k=1 ensures a single result; truncation handles long texts
        result = sentiment_pipeline(text[:512], top_k=1)[0]
    except Exception as e:
        logger.error("Pipeline error: %s", e, exc_info=True)
        raise

    raw_label = result["label"]
    score = round(result["score"], 4)

    # Normalize label to Positive/Negative/Neutral regardless of casing
    label = LABEL_MAP.get(raw_label.lower(), "Neutral")

    logger.info(
        "Analysis done: label=%s, score=%.4f (raw: %s)",
        label,
        score,
        raw_label,
    )

    return {"label": label, "score": score}
