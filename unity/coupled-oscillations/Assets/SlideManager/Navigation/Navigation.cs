﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Navigation : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject backButton = default;
    [SerializeField] private GameObject forwardButton = default;

    [Header("Bubbles")]
    [SerializeField] private Sprite filledDisk = default;
    [SerializeField] private Sprite openCircle = default;
    [SerializeField] private Color pastColor = Color.black;
    [SerializeField] private Color currentColor = Color.red;
    [SerializeField] private Color futureColor = Color.white;
    [SerializeField] private bool bubblesAreClickable = false;

    [Header("Progress Bar")]
    [SerializeField] private RectTransform progressBarBackground = default;
    [SerializeField] private RectTransform progressBarFill = default;
    [HideInInspector] public bool useProgressBar;
    private int numSlides;

    private RectTransform bubbles;
    private int currentSlideIndex;

    public void SetNumSlides(int numSlides)
    {
        this.numSlides = numSlides;
    }

    public void GenerateBubbles(int numSlides)
    {
        bubbles = (RectTransform)transform.Find("Bubbles");
        if (bubbles == null)
        {
            Debug.LogWarning("Navigation could not find a child GameObject called Bubbles");
            return;
        }

        for (int i = 0; i < numSlides; i++)
        {
            RectTransform bubbleTransform = new GameObject("Bubble" + i, typeof(Image)).GetComponent<RectTransform>();
            bubbleTransform.gameObject.AddComponent<Bubble>();
            Image bubbleImage = bubbleTransform.GetComponent<Image>();
            bubbleTransform.SetParent(bubbles);
            bubbleTransform.anchoredPosition = Vector2.zero;
            bubbleTransform.sizeDelta = bubbles.sizeDelta.y * Vector2.one;
            bubbleTransform.localScale = Vector3.one;
            bubbleImage.color -= new Color(0, 0, 0, bubbleImage.color.a);
            bubbleImage.preserveAspect = true;
        }

        SetNumSlides(numSlides);
    }

    private void SetActiveBubble(int slideIndex)
    {
        if (bubbles == null || filledDisk == null || openCircle == null)
        {
            Debug.LogWarning("Navigation > must assign Filled Disk and Open Circle sprites.");
            return;
        }

        for (int i = 0; i < bubbles.childCount; i++)
        {
            Image image = bubbles.GetChild(i).GetComponent<Image>();
            if (i < slideIndex)
            {
                image.sprite = filledDisk;
                image.color = pastColor;
            }
            else if (i == slideIndex)
            {
                image.sprite = filledDisk;
                image.color = currentColor;
            }
            else
            {
                image.sprite = openCircle;
                image.color = futureColor;
            }
        }
    }

    public void FillProgressBar(int slideIndex)
    {
        if (progressBarBackground && progressBarFill)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateFill((slideIndex + 1f) / numSlides));
        }
    }

    private IEnumerator AnimateFill(float targetFraction, float lerpTime = 0.5f)
    {
        float currentFraction = progressBarFill.sizeDelta.x / progressBarBackground.rect.size.x;

        float time = 0;
        Vector2 sizeDelta = progressBarFill.sizeDelta;

        while (time < lerpTime)
        {
            time += Time.deltaTime;
            float t = time / lerpTime;
            t = t * t * (3f - 2f * t);
            float fraction = Mathf.Lerp(currentFraction, targetFraction, t);
            sizeDelta.x = fraction * progressBarBackground.rect.size.x;
            progressBarFill.sizeDelta = sizeDelta;
            yield return null;
        }

        sizeDelta.x = targetFraction * progressBarBackground.rect.size.x;
        progressBarFill.sizeDelta = sizeDelta;
    }

    private void SetButtonVisibility()
    {
        if (backButton == null || forwardButton == null)
        {
            Debug.LogWarning("No navigation arrows assigned.");
            return;
        }

        if (currentSlideIndex == 0)
        {
            backButton.SetActive(false);
            forwardButton.SetActive(numSlides > 1);
        }
        else if (currentSlideIndex == numSlides - 1)
        {
            backButton.SetActive(true);
            forwardButton.SetActive(false);
        }
        else
        {
            backButton.SetActive(true);
            forwardButton.SetActive(true);
        }
    }

    public void ChangeSlide(int slideIndex, bool sendMessage = true)
    {
        if (useProgressBar)
        {
            FillProgressBar(slideIndex);
        }
        else
        {
            SetActiveBubble(slideIndex);
        }

        SetButtonVisibility();
        if (sendMessage)
        {
            SendMessageUpwards("LoadSlide", slideIndex, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void GoBack()
    {
        currentSlideIndex--;
        ChangeSlide(currentSlideIndex);
    }

    public void GoForward()
    {
        currentSlideIndex++;
        ChangeSlide(currentSlideIndex);
    }

    public void SetBubbleClickability(bool clickable)
    {
        bubblesAreClickable = clickable;
    }

    public void HandleBubbleClick(int slideIndex)
    {
        if (!bubblesAreClickable) { return; }

        if (currentSlideIndex != slideIndex)
        {
            currentSlideIndex = slideIndex;
            ChangeSlide(currentSlideIndex);
        }
    }

    public void SetCurrentSlideIndex(int slideIndex)
    {
        currentSlideIndex = slideIndex;
    }
}
