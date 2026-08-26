// ============================================================
//  STUDENT MANAGEMENT SYSTEM — SITE JS
// ============================================================

(function () {
    'use strict';

    /* ── Page load animations ── */
    document.addEventListener('DOMContentLoaded', function () {

        // Animate main content in
        document.querySelectorAll('.animate-on-load').forEach(function (el, i) {
            el.style.animationDelay = (i * 0.08) + 's';
            el.classList.add('animate-fade-up');
        });

        // Navbar scroll effect
        var navbar = document.querySelector('.navbar');
        if (navbar) {
            window.addEventListener('scroll', function () {
                if (window.scrollY > 10) {
                    navbar.style.boxShadow = '0 8px 32px rgba(79,70,229,0.18)';
                } else {
                    navbar.style.boxShadow = '0 4px 24px rgba(79,70,229,0.10)';
                }
            });
        }

        // Photo preview before upload
        var fileInputs = document.querySelectorAll('input[type="file"]');
        fileInputs.forEach(function (input) {
            input.addEventListener('change', function (e) {
                var file = e.target.files[0];
                if (!file) return;
                if (!file.type.startsWith('image/')) return;

                var reader = new FileReader();
                reader.onload = function (ev) {
                    // find closest preview img
                    var form = input.closest('form');
                    if (!form) return;
                    var preview = form.querySelector('img');
                    if (preview) {
                        preview.style.opacity = '0';
                        preview.style.transition = 'opacity 0.3s ease';
                        setTimeout(function () {
                            preview.src = ev.target.result;
                            preview.style.opacity = '1';
                        }, 150);
                    }
                };
                reader.readAsDataURL(file);
            });
        });

        // Confirm delete
        var deleteForms = document.querySelectorAll('form[data-confirm]');
        deleteForms.forEach(function (form) {
            form.addEventListener('submit', function (e) {
                if (!confirm(form.dataset.confirm || 'Are you sure?')) {
                    e.preventDefault();
                }
            });
        });

        // Auto-dismiss alerts after 4s
        document.querySelectorAll('.alert-auto-dismiss').forEach(function (el) {
            setTimeout(function () {
                el.style.transition = 'opacity 0.5s ease';
                el.style.opacity = '0';
                setTimeout(function () { el.remove(); }, 500);
            }, 4000);
        });

        // Table row hover highlight
        document.querySelectorAll('.table tbody tr').forEach(function (row) {
            row.style.cursor = 'default';
        });

        // Tooltip on student photos
        document.querySelectorAll('.student-photo').forEach(function (img) {
            img.title = 'Click to enlarge';
            img.style.cursor = 'zoom-in';
            img.addEventListener('click', function () {
                openLightbox(img.src, img.alt || 'Student Photo');
            });
        });

        // Smooth scroll for any in-page anchors
        document.querySelectorAll('a[href^="#"]').forEach(function (a) {
            a.addEventListener('click', function (e) {
                var target = document.querySelector(a.getAttribute('href'));
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                }
            });
        });

        // Input focus effect - add glow class
        document.querySelectorAll('.form-control, .form-select').forEach(function (el) {
            el.addEventListener('focus', function () {
                el.closest('.form-group, .mb-3')?.classList.add('focused');
            });
            el.addEventListener('blur', function () {
                el.closest('.form-group, .mb-3')?.classList.remove('focused');
            });
        });

        // Bootstrap tooltips if available
        if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
            document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
                new bootstrap.Tooltip(el);
            });
        }

        // Animate table rows
        document.querySelectorAll('.table tbody tr').forEach(function (row, i) {
            row.style.animationDelay = (i * 0.04) + 's';
            row.classList.add('animate-fade-up');
        });

        // Add icons to action links
        document.querySelectorAll('.link-edit').forEach(function (a) {
            if (!a.querySelector('.icon')) a.insertAdjacentHTML('afterbegin', '<span class="icon">✏️</span>');
        });
        document.querySelectorAll('.link-details').forEach(function (a) {
            if (!a.querySelector('.icon')) a.insertAdjacentHTML('afterbegin', '<span class="icon">👁</span>');
        });
        document.querySelectorAll('.link-delete').forEach(function (a) {
            if (!a.querySelector('.icon')) a.insertAdjacentHTML('afterbegin', '<span class="icon">🗑</span>');
        });

    });

    /* ── Lightbox ── */
    function openLightbox(src, alt) {
        var overlay = document.createElement('div');
        overlay.style.cssText = [
            'position:fixed;inset:0;z-index:9999;',
            'background:rgba(0,0,0,0.85);',
            'display:flex;align-items:center;justify-content:center;',
            'animation:fadeIn 0.25s ease;cursor:zoom-out;',
            'backdrop-filter:blur(6px);'
        ].join('');

        var img = document.createElement('img');
        img.src = src;
        img.alt = alt;
        img.style.cssText = [
            'max-width:85vw;max-height:85vh;',
            'border-radius:14px;',
            'box-shadow:0 24px 80px rgba(0,0,0,0.6);',
            'animation:fadeInUp 0.3s ease;'
        ].join('');

        overlay.appendChild(img);
        document.body.appendChild(overlay);

        overlay.addEventListener('click', function () {
            overlay.style.opacity = '0';
            overlay.style.transition = 'opacity 0.2s ease';
            setTimeout(function () { overlay.remove(); }, 200);
        });

        document.addEventListener('keydown', function esc(e) {
            if (e.key === 'Escape') { overlay.click(); document.removeEventListener('keydown', esc); }
        });
    }

    /* ── Utility: format date strings ── */
    window.formatDate = function (dateStr) {
        if (!dateStr) return '';
        var d = new Date(dateStr);
        return d.toLocaleDateString('en-ZA', { year: 'numeric', month: 'short', day: 'numeric' });
    };

})();

    /* ── Error sound on validation failure ── */
    (function () {
        // Play a dip/buzz sound when login/register has errors
        function playErrorSound() {
            try {
                var ctx = new (window.AudioContext || window.webkitAudioContext)();
                var osc = ctx.createOscillator();
                var gain = ctx.createGain();
                osc.connect(gain);
                gain.connect(ctx.destination);
                osc.type = 'sine';
                osc.frequency.setValueAtTime(420, ctx.currentTime);
                osc.frequency.exponentialRampToValueAtTime(180, ctx.currentTime + 0.18);
                gain.gain.setValueAtTime(0.35, ctx.currentTime);
                gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.28);
                osc.start(ctx.currentTime);
                osc.stop(ctx.currentTime + 0.3);
            } catch (e) { /* audio not available */ }
        }

        function shakeElement(el) {
            el.style.animation = 'none';
            el.offsetHeight; // reflow
            el.style.animation = 'shake 0.4s ease';
        }

        document.addEventListener('DOMContentLoaded', function () {
            // Watch for server-side error (alert-danger already present on load)
            var errAlert = document.querySelector('.alert-danger');
            if (errAlert && errAlert.textContent.trim().length > 0) {
                playErrorSound();
                var card = document.querySelector('.form-card');
                if (card) shakeElement(card);
            }

            // Client-side: play sound when form submits but has validation errors
            var forms = document.querySelectorAll('form#account');
            forms.forEach(function (form) {
                form.addEventListener('submit', function () {
                    // Delay to let unobtrusive validation run first
                    setTimeout(function () {
                        var visible = form.querySelectorAll('.field-validation-error, .validation-summary-errors li');
                        if (visible.length > 0) {
                            playErrorSound();
                            var card = form.closest('.form-card');
                            if (card) shakeElement(card);
                        }
                    }, 60);
                });
            });
        });
    })();
