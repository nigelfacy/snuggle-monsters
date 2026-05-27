// <copyright file="MonsterAnimatorController.cs" company="SnuggleMonsters">
// Copyright (c) SnuggleMonsters. All rights reserved.
// </copyright>

using System;
using System.Collections;
using UnityEngine;

namespace SnuggleMonsters.Monster
{
    /// <summary>
    /// Controls the monster's placeholder animations using coroutines.
    /// All animation is done via simple Lerp-based tweens in coroutines —
    /// no Animator Controller, no DOTween dependency.
    /// Uses only <see cref="Mathf.Lerp"/> and <see cref="Transform"/> manipulation.
    /// </summary>
    /// <remarks>
    /// This is a prototype animation system. In the final game, these would be
    /// replaced with proper Mecanim Animator states or timeline assets.
    /// </remarks>
    public class MonsterAnimatorController : MonoBehaviour
    {
        // ----- Serialised References -----

        [Header("Monster Transforms")]

        /// <summary>Root transform for the entire monster model. Used for squish/stretch.</summary>
        [SerializeField]
        [Tooltip("Root transform of the monster. Used for bounce/squish effects.")]
        private Transform monsterRoot;

        /// <summary>Transform for the eyes. Scaled for blink animation.</summary>
        [SerializeField]
        [Tooltip("Transform of the monster's eyes. Used for blink animation.")]
        private Transform eyesTransform;

        /// <summary>Transform for an arm/wing used in wave animation.</summary>
        [SerializeField]
        [Tooltip("Transform of an arm or wing used for waving animation.")]
        private Transform armTransform;

        /// <summary>Transform used for gentle lullaby rocking motion.</summary>
        [SerializeField]
        [Tooltip("Transform used for lullaby rocking / side-to-side sway.")]
        private Transform rockTransform;

        [Header("Timing")]

        /// <summary>Duration of a single blink in seconds.</summary>
        [SerializeField]
        [Tooltip("Duration of a single blink (squish and recover).")]
        private float blinkDuration = 0.15f;

        /// <summary>Duration of a full bounce cycle in seconds.</summary>
        [SerializeField]
        [Tooltip("Duration of one bounce cycle.")]
        private float bounceDuration = 0.4f;

        /// <summary>Amount of Y-scale squish during bounce (1.0 = normal).</summary>
        [SerializeField]
        [Tooltip("Scale multiplier for the squish part of a bounce (lower = more squish).")]
        private float bounceSquishAmount = 0.8f;

        // ----- Private State -----

        /// <summary>Stored original local scales for proper reset after animations.</summary>
        private Vector3 rootOriginalScale;
        private Vector3 eyesOriginalScale;
        private Vector3 armOriginalRotation;
        private Vector3 rockOriginalPosition;

        /// <summary>Currently playing coroutine reference, to allow interruption.</summary>
        private Coroutine currentAnimation;

        // ----- Lifecycle -----

        private void Awake()
        {
            // Store original transforms for reset
            if (monsterRoot != null)
            {
                rootOriginalScale = monsterRoot.localScale;
            }

            if (eyesTransform != null)
            {
                eyesOriginalScale = eyesTransform.localScale;
            }

            if (armTransform != null)
            {
                armOriginalRotation = armTransform.localEulerAngles;
            }

            if (rockTransform != null)
            {
                rockOriginalPosition = rockTransform.localPosition;
            }
        }

        // ----- Animation Coroutines -----

        /// <summary>
        /// Plays a blink animation by scaling the eyes down on Y and back up.
        /// </summary>
        /// <returns>IEnumerator for coroutine usage.</returns>
        public IEnumerator Blink()
        {
            if (eyesTransform == null)
            {
                // TODO: Play blink sound effect
                yield break;
            }

            // TODO: Play blink sound effect — placeholder for audio hook

            Vector3 originalScale = eyesTransform.localScale;
            Vector3 closedScale = new Vector3(originalScale.x, originalScale.y * 0.1f, originalScale.z);

            // Close eyes
            yield return LerpScale(eyesTransform, originalScale, closedScale, blinkDuration * 0.5f);

            // Open eyes
            yield return LerpScale(eyesTransform, closedScale, originalScale, blinkDuration * 0.5f);
        }

        /// <summary>
        /// Plays a happy bounce by squishing the monster root on Y and recovering.
        /// </summary>
        /// <returns>IEnumerator for coroutine usage.</returns>
        public IEnumerator BounceHappy()
        {
            if (monsterRoot == null)
            {
                yield break;
            }

            // TODO: Play happy bounce sound effect — placeholder for audio hook

            Vector3 squishScale = new Vector3(
                rootOriginalScale.x * (2f - bounceSquishAmount), // slight X stretch
                rootOriginalScale.y * bounceSquishAmount,         // Y squish
                rootOriginalScale.z);

            // Squish down
            yield return LerpScale(monsterRoot, rootOriginalScale, squishScale, bounceDuration * 0.4f);

            // Recover with slight overshoot
            yield return LerpScale(monsterRoot, squishScale, rootOriginalScale, bounceDuration * 0.6f);

            // Ensure exact reset
            monsterRoot.localScale = rootOriginalScale;
        }

        /// <summary>
        /// Plays a waving animation by rotating the arm transform back and forth.
        /// </summary>
        /// <returns>IEnumerator for coroutine usage.</returns>
        public IEnumerator Wave()
        {
            if (armTransform == null)
            {
                yield break;
            }

            // TODO: Play wave sound effect — placeholder for audio hook

            Vector3 startRotation = armOriginalRotation;
            Vector3 waveLeft = startRotation + new Vector3(0f, 0f, -30f);
            Vector3 waveRight = startRotation + new Vector3(0f, 0f, 30f);

            float waveSpeed = 0.15f;
            int waves = 3;

            for (int i = 0; i < waves; i++)
            {
                yield return LerpEuler(armTransform, startRotation, waveRight, waveSpeed);
                yield return LerpEuler(armTransform, waveRight, waveLeft, waveSpeed * 2f);
                yield return LerpEuler(armTransform, waveLeft, startRotation, waveSpeed);
            }

            // Ensure exact reset
            armTransform.localEulerAngles = armOriginalRotation;
        }

        /// <summary>
        /// Plays a dance sequence based on the specified style and speed.
        /// Combines bounce, spin, and wiggle sub-sequences.
        /// </summary>
        /// <param name="style">Dance style name (e.g. "Wiggly", "Spinny", "Bouncy").</param>
        /// <param name="speed">Speed multiplier (1.0 = normal).</param>
        /// <returns>IEnumerator for coroutine usage.</returns>
        public IEnumerator Dance(string style, float speed)
        {
            if (monsterRoot == null)
            {
                yield break;
            }

            // Sanitise inputs
            speed = Mathf.Max(0.1f, speed);

            // TODO: Play dance music/sfx — placeholder for audio hook
            Debug.Log($"[MonsterAnimatorController] Dancing: {style} at speed {speed}");

            float danceDuration = 2f / speed;
            float elapsed = 0f;

            switch (style.ToLowerInvariant())
            {
                case "wiggly":
                    // Side-to-side wiggle with bounce
                    while (elapsed < danceDuration)
                    {
                        float phase = Mathf.Sin(elapsed * 12f * speed) * 15f;
                        monsterRoot.localEulerAngles = new Vector3(0f, 0f, phase);
                        monsterRoot.localScale = rootOriginalScale +
                            new Vector3(0f, Mathf.Sin(elapsed * 8f * speed) * 0.05f, 0f);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }

                    break;

                case "spinny":
                    // Full rotation spins
                    while (elapsed < danceDuration)
                    {
                        float rotation = Mathf.Lerp(0f, 360f * 2f, elapsed / danceDuration);
                        monsterRoot.localEulerAngles = new Vector3(0f, rotation, 0f);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }

                    break;

                case "bouncy":
                    // Repeated happy bounces at an accelerated rate
                    float bounceInterval = bounceDuration / speed;
                    int bounceCount = Mathf.FloorToInt(danceDuration / bounceInterval);

                    for (int i = 0; i < bounceCount; i++)
                    {
                        // Quick squish
                        Vector3 squish = new Vector3(
                            rootOriginalScale.x * 1.1f,
                            rootOriginalScale.y * 0.85f,
                            rootOriginalScale.z);
                        yield return LerpScale(monsterRoot, rootOriginalScale, squish, bounceInterval * 0.3f);
                        yield return LerpScale(monsterRoot, squish, rootOriginalScale, bounceInterval * 0.7f);
                    }

                    break;

                default:
                    // Generic dance: gentle sway with small bounce
                    while (elapsed < danceDuration)
                    {
                        float sway = Mathf.Sin(elapsed * 6f * speed) * 10f;
                        monsterRoot.localEulerAngles = new Vector3(0f, 0f, sway);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }

                    break;
            }

            // Reset to original state
            monsterRoot.localScale = rootOriginalScale;
            monsterRoot.localEulerAngles = Vector3.zero;

            // TODO: Play dance finish fanfare — placeholder for audio hook
        }

        /// <summary>
        /// Plays a sleeping animation with gentle breathing scale oscillation.
        /// </summary>
        /// <returns>IEnumerator for coroutine usage. Runs until stopped externally.</returns>
        public IEnumerator SleepAnim()
        {
            if (monsterRoot == null)
            {
                yield break;
            }

            // TODO: Play gentle breathing sound effect — placeholder for audio hook

            float breathDuration = 3f; // seconds per full breath cycle
            float breathAmount = 0.03f;

            while (true)
            {
                // Slow in-out breathing using a sine wave
                float t = (Time.time % breathDuration) / breathDuration;
                float breathScale = 1f + Mathf.Sin(t * Mathf.PI * 2f) * breathAmount;

                monsterRoot.localScale = new Vector3(
                    rootOriginalScale.x * breathScale,
                    rootOriginalScale.y,
                    rootOriginalScale.z * breathScale);

                yield return null;
            }

            // ReSharper disable once IteratorNeverReturns — this runs until interrupted
        }

        /// <summary>
        /// Plays a gentle side-to-side lullaby rocking animation.
        /// </summary>
        /// <returns>IEnumerator for coroutine usage. Runs until stopped externally.</returns>
        public IEnumerator LullabyRock()
        {
            if (rockTransform == null)
            {
                yield break;
            }

            // TODO: Play lullaby music — placeholder for audio hook

            float rockDuration = 4f;
            float rockAngle = 10f;

            while (true)
            {
                float t = (Time.time % rockDuration) / rockDuration;
                float angle = Mathf.Sin(t * Mathf.PI * 2f) * rockAngle;

                rockTransform.localEulerAngles = new Vector3(0f, 0f, angle);

                yield return null;
            }

            // ReSharper disable once IteratorNeverReturns — this runs until interrupted
        }

        /// <summary>
        /// Plays a sparkle effect. If a ParticleSystem is available, triggers a burst.
        /// Otherwise falls back to a simple scale flash.
        /// </summary>
        /// <param name="intensity">Effect intensity (0.0 – 1.0).</param>
        /// <returns>IEnumerator for coroutine usage.</returns>
        public IEnumerator SparkleEffect(float intensity)
        {
            intensity = Mathf.Clamp01(intensity);

            // Try particle system first
            // TODO: Replace with proper particle effect once VFX art assets are available
            ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
            if (particles != null)
            {
                var emission = particles.emission;
                emission.enabled = true;
                particles.Play();

                float playTime = Mathf.Lerp(0.3f, 1.5f, intensity);
                yield return new WaitForSeconds(playTime);

                particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                yield break;
            }

            // Fallback: simple scale flash
            if (monsterRoot == null)
            {
                yield break;
            }

            float flashScale = 1f + (intensity * 0.3f);
            Vector3 targetScale = rootOriginalScale * flashScale;

            // Pulse in
            yield return LerpScale(monsterRoot, rootOriginalScale, targetScale, 0.1f);

            // Pulse out
            yield return LerpScale(monsterRoot, targetScale, rootOriginalScale, 0.2f);

            // Second quick pulse for emphasis
            yield return LerpScale(monsterRoot, rootOriginalScale, targetScale * 1.1f, 0.08f);
            yield return LerpScale(monsterRoot, targetScale * 1.1f, rootOriginalScale, 0.12f);

            monsterRoot.localScale = rootOriginalScale;
        }

        /// <summary>
        /// Stops any currently playing animation coroutine.
        /// Resets all transforms to their originally stored values.
        /// </summary>
        public void StopAllAnimations()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }

            // Reset all transforms
            if (monsterRoot != null)
            {
                monsterRoot.localScale = rootOriginalScale;
                monsterRoot.localEulerAngles = Vector3.zero;
            }

            if (eyesTransform != null)
            {
                eyesTransform.localScale = eyesOriginalScale;
            }

            if (armTransform != null)
            {
                armTransform.localEulerAngles = armOriginalRotation;
            }

            if (rockTransform != null)
            {
                rockTransform.localPosition = rockOriginalPosition;
                rockTransform.localEulerAngles = Vector3.zero;
            }
        }

        // ----- Helper Tweens -----

        /// <summary>
        /// Interpolates a transform's local scale from start to end over a duration.
        /// </summary>
        private static IEnumerator LerpScale(Transform target, Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                target.localScale = Vector3.Lerp(from, to, t);
                yield return null;
            }

            target.localScale = to;
        }

        /// <summary>
        /// Interpolates a transform's local euler angles from start to end over a duration.
        /// </summary>
        private static IEnumerator LerpEuler(Transform target, Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                target.localEulerAngles = Vector3.Lerp(from, to, t);
                yield return null;
            }

            target.localEulerAngles = to;
        }

        /// <summary>
        /// Interpolates a transform's local position from start to end over a duration.
        /// </summary>
        private static IEnumerator LerpPosition(Transform target, Vector3 from, Vector3 to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                target.localPosition = Vector3.Lerp(from, to, t);
                yield return null;
            }

            target.localPosition = to;
        }

        // ----- Editor Validation -----

        private void OnValidate()
        {
            blinkDuration = Mathf.Max(0.05f, blinkDuration);
            bounceDuration = Mathf.Max(0.1f, bounceDuration);
            bounceSquishAmount = Mathf.Clamp(bounceSquishAmount, 0.1f, 1f);
        }
    }
}