// Configuração de velocidade da animação dos indicadores numéricos (em ms)
export const STATS_ANIMATION_DURATION_MS = 1800;

let statsObserver = null;
let activeRafIds = new Set();
let mediaQueryList = null;
let mediaQueryHandler = null;

export function isReducedMotion() {
    return typeof window !== "undefined" &&
        typeof window.matchMedia === "function" &&
        window.matchMedia("(prefers-reduced-motion: reduce)").matches;
}

function formatStatValue(value, decimals) {
    return Number(value).toFixed(decimals);
}

function completeAllCountersImmediately(sectionElement) {
    if (!sectionElement) {
        return;
    }

    const counters = sectionElement.querySelectorAll(".stat-counter");
    counters.forEach((counter) => {
        const target = parseFloat(counter.getAttribute("data-target") || "0");
        const decimals = parseInt(counter.getAttribute("data-decimals") || "0", 10);
        counter.textContent = formatStatValue(target, decimals);
        counter.setAttribute("data-animated", "true");
    });
}

function animateCounter(counterElement, durationMs) {
    if (!counterElement || counterElement.getAttribute("data-animated") === "true") {
        return;
    }

    counterElement.setAttribute("data-animated", "true");

    const target = parseFloat(counterElement.getAttribute("data-target") || "0");
    const decimals = parseInt(counterElement.getAttribute("data-decimals") || "0", 10);

    if (isReducedMotion() || durationMs <= 0 || isNaN(target)) {
        counterElement.textContent = formatStatValue(target, decimals);
        return;
    }

    counterElement.textContent = formatStatValue(0, decimals);
    let startTime = null;
    let rafId = 0;

    const step = (timestamp) => {
        if (startTime === null) {
            startTime = timestamp;
        }

        const elapsed = timestamp - startTime;
        const progress = Math.min(elapsed / durationMs, 1);
        // Desaceleração suave perto do final (cubic ease-out)
        const eased = 1 - Math.pow(1 - progress, 3);
        const currentValue = target * eased;

        if (progress < 1) {
            counterElement.textContent = formatStatValue(currentValue, decimals);
            rafId = window.requestAnimationFrame(step);
            activeRafIds.add(rafId);
        } else {
            counterElement.textContent = formatStatValue(target, decimals);
            activeRafIds.delete(rafId);
        }
    };

    rafId = window.requestAnimationFrame(step);
    activeRafIds.add(rafId);
}

export function initHomeInteractions(statsSectionElement, dotNetRef, durationMs = STATS_ANIMATION_DURATION_MS) {
    disposeHomeInteractions();

    const effectiveDuration = typeof durationMs === "number" && durationMs > 0
        ? durationMs
        : STATS_ANIMATION_DURATION_MS;

    if (typeof window !== "undefined" && typeof window.matchMedia === "function") {
        mediaQueryList = window.matchMedia("(prefers-reduced-motion: reduce)");
        mediaQueryHandler = (event) => {
            if (event.matches && statsSectionElement) {
                activeRafIds.forEach((id) => window.cancelAnimationFrame(id));
                activeRafIds.clear();
                completeAllCountersImmediately(statsSectionElement);
            }
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync("OnReducedMotionChanged", event.matches).catch(() => {});
            }
        };

        if (typeof mediaQueryList.addEventListener === "function") {
            mediaQueryList.addEventListener("change", mediaQueryHandler);
        }
    }

    if (!statsSectionElement) {
        return;
    }

    if (isReducedMotion() || typeof IntersectionObserver === "undefined") {
        completeAllCountersImmediately(statsSectionElement);
        return;
    }

    const counters = statsSectionElement.querySelectorAll(".stat-counter");
    counters.forEach((counter) => {
        if (counter.getAttribute("data-animated") !== "true") {
            const decimals = parseInt(counter.getAttribute("data-decimals") || "0", 10);
            counter.textContent = formatStatValue(0, decimals);
        }
    });

    statsObserver = new IntersectionObserver(
        (entries, observer) => {
            entries.forEach((entry) => {
                if (!entry.isIntersecting) {
                    return;
                }

                const sectionCounters = entry.target.querySelectorAll(".stat-counter");
                sectionCounters.forEach((counter) => {
                    animateCounter(counter, effectiveDuration);
                });

                observer.unobserve(entry.target);
            });
        },
        { threshold: 0.2 }
    );

    statsObserver.observe(statsSectionElement);
}

export function disposeHomeInteractions() {
    if (statsObserver) {
        statsObserver.disconnect();
        statsObserver = null;
    }

    if (typeof window !== "undefined" && typeof window.cancelAnimationFrame === "function") {
        activeRafIds.forEach((id) => window.cancelAnimationFrame(id));
    }
    activeRafIds.clear();

    if (mediaQueryList && mediaQueryHandler && typeof mediaQueryList.removeEventListener === "function") {
        mediaQueryList.removeEventListener("change", mediaQueryHandler);
    }
    mediaQueryList = null;
    mediaQueryHandler = null;
}

