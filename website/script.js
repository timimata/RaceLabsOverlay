// Mobile menu toggle
const mobileMenuBtn = document.querySelector('.mobile-menu-btn');
const navLinks = document.querySelector('.nav-links');

if (mobileMenuBtn) {
    mobileMenuBtn.addEventListener('click', () => {
        navLinks.style.display = navLinks.style.display === 'flex' ? 'none' : 'flex';
    });
}

// Smooth scroll for navigation links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Navbar background on scroll
const navbar = document.querySelector('.navbar');
let lastScroll = 0;

window.addEventListener('scroll', () => {
    const currentScroll = window.pageYOffset;
    
    if (currentScroll > 100) {
        navbar.style.background = 'rgba(10, 10, 10, 0.98)';
    } else {
        navbar.style.background = 'rgba(10, 10, 10, 0.95)';
    }
    
    lastScroll = currentScroll;
});

// Animate elements on scroll
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
};

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.style.opacity = '1';
            entry.target.style.transform = 'translateY(0)';
        }
    });
}, observerOptions);

// Observe feature cards and widget items
document.querySelectorAll('.feature-card, .widget-item').forEach((el, index) => {
    el.style.opacity = '0';
    el.style.transform = 'translateY(20px)';
    el.style.transition = `opacity 0.5s ease ${index * 0.1}s, transform 0.5s ease ${index * 0.1}s`;
    observer.observe(el);
});

// Dynamic year in footer
const yearSpan = document.querySelector('.footer-bottom p:last-child');
if (yearSpan) {
    const currentYear = new Date().getFullYear();
    yearSpan.innerHTML = `© ${currentYear} Tiago. Open source under MIT License.`;
}

// Simulate live widget data in hero section
function updateWidgetDemo() {
    const speedometer = document.querySelector('.widget.speedometer .widget-value');
    const deltaValue = document.querySelector('.widget.delta .delta-value');
    
    if (speedometer && deltaValue) {
        // Simulate speed fluctuation
        const baseSpeed = 240;
        const speedVariation = Math.floor(Math.random() * 10);
        speedometer.textContent = baseSpeed + speedVariation;
        
        // Simulate delta fluctuation
        const delta = (-0.200 - Math.random() * 0.100).toFixed(3);
        deltaValue.textContent = delta;
        deltaValue.style.color = delta < 0 ? '#00ff00' : '#ff3333';
    }
}

// Update demo every 2 seconds
setInterval(updateWidgetDemo, 2000);

// Download button tracking
document.querySelectorAll('.btn-download').forEach(btn => {
    btn.addEventListener('click', () => {
        console.log('Download clicked');
        // Here you could add analytics tracking
    });
});

// Add loading animation for page
window.addEventListener('load', () => {
    document.body.classList.add('loaded');
});

// Keyboard shortcuts info
console.log('%c🏎️ RaceLabs Overlay', 'font-size: 24px; font-weight: bold; color: #00ff88;');
console.log('%cProfessional iRacing Telemetry', 'font-size: 14px; color: #888;');
console.log('%cVisit: https://github.com/timimata/RaceLabsOverlay', 'font-size: 12px; color: #666;');
