// --- State ---
let isOpen = false;
let isSending = false;
let welcomeShown = false;
let lastUserQuery = "";
let heartbeatTimer = null;

// Elements
const dlg = document.getElementById('asdcChat');
const toggleBtn = document.getElementById('asdcChatToggle');
const closeBtn = document.getElementById('asdcClose');
const chatBox = document.getElementById('asdcChatBox');
const inputEl = document.getElementById('asdcQuery');
const sendBtn = document.getElementById('asdcSend');
const loadingEl = document.getElementById('asdcLoading');

// Focus-trap support
function focusableElements(root) {
    return Array.from(root.querySelectorAll(
        'button, [href], input, textarea, select, [tabindex]:not([tabindex="-1"])'
    )).filter(el => !el.disabled && el.offsetParent !== null);
}

function openChat() {
    if (isOpen) return;
    dlg.hidden = false;
    toggleBtn.setAttribute('aria-expanded', 'true');
    isOpen = true;

    // First-time welcome
    if (!welcomeShown) {
        showWelcome();
        welcomeShown = true;
    }

    // Start heartbeat while open (every 4 min)
    if (!heartbeatTimer) {
        heartbeatTimer = setInterval(() => {
            sendQuery('hi', true);
        }, 240000);
    }

    // Focus management
    const f = focusableElements(dlg);
    if (f.length) f[0].focus();

    // Scroll to last
    requestAnimationFrame(() => {
        chatBox.scrollTop = chatBox.scrollHeight;
    });

    // Close on ESC
    document.addEventListener('keydown', onEsc, { capture: true });
    dlg.addEventListener('keydown', trapFocus);
}

function closeChat() {
    if (!isOpen) return;
    dlg.hidden = true;
    toggleBtn.setAttribute('aria-expanded', 'false');
    isOpen = false;

    // Stop heartbeat
    if (heartbeatTimer) {
        clearInterval(heartbeatTimer);
        heartbeatTimer = null;
    }

    // Return focus to toggle
    toggleBtn.focus();

    document.removeEventListener('keydown', onEsc, { capture: true });
    dlg.removeEventListener('keydown', trapFocus);
}

function onEsc(e) {
    if (e.key === 'Escape') {
        closeChat();
        e.preventDefault();
    }
}

function trapFocus(e) {
    if (e.key !== 'Tab') return;
    const f = focusableElements(dlg);
    if (!f.length) return;
    const first = f[0];
    const last = f[f.length - 1];
    if (e.shiftKey && document.activeElement === first) {
        last.focus();
        e.preventDefault();
    } else if (!e.shiftKey && document.activeElement === last) {
        first.focus();
        e.preventDefault();
    }
}

// Toggle
toggleBtn.addEventListener('click', () => (isOpen ? closeChat() : openChat()));
closeBtn.addEventListener('click', closeChat);

// Submit handlers
sendBtn.addEventListener('click', () => sendQuery());
inputEl.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') sendQuery();
});

// Dynamic viewport height var (mobile keyboards)
function setVhVar() {
    const vh = window.visualViewport ? window.visualViewport.height : window.innerHeight;
    document.documentElement.style.setProperty('--asdc-vh', vh + 'px');
}
setVhVar();
window.addEventListener('resize', setVhVar);
if (window.visualViewport) window.visualViewport.addEventListener('resize', setVhVar);

// --- UI helpers (vanilla, no Tailwind) ---
function scrollToBottom(el, smooth = true) {
    const nearBottom = el.scrollHeight - el.scrollTop - el.clientHeight < 80;
    el.scrollTo({ top: el.scrollHeight, behavior: smooth && nearBottom ? 'smooth' : 'auto' });
}

function streamText(container, text, speed = 25, onDone, onChunk) {
    let i = 0;
    (function tick() {
        if (i < text.length) {
            const ch = text[i] === "\n" ? "<br>" : text[i];
            container.innerHTML += ch;
            if (onChunk) onChunk();
            i++;
            setTimeout(tick, speed);
        } else {
            if (onDone) onDone();
        }
    })();
}

function appendRating(container) {
    const feedbackDiv = document.createElement('div');
    feedbackDiv.className = 'asdc-feedback';

    const like = document.createElement('button');
    like.type = 'button';
    like.className = 'asdc-icon-btn';
    like.title = 'Like';
    like.textContent = '👍';

    const dislike = document.createElement('button');
    dislike.type = 'button';
    dislike.className = 'asdc-icon-btn';
    dislike.title = 'Dislike';
    dislike.textContent = '👎';

    feedbackDiv.appendChild(like);
    feedbackDiv.appendChild(dislike);
    container.appendChild(feedbackDiv);

    feedbackDiv.addEventListener('click', (ev) => {
        if (ev.target === like) rateResponse('like');
        if (ev.target === dislike) rateResponse('dislike');
    });
}

function addMessage(role, text, options = {}) {
    const { stream = false, link = "", meta = null } = options;

    const row = document.createElement('div');
    row.className = role === 'user' ? 'asdc-row asdc-row--me' : 'asdc-row asdc-row--bot';

    const icon = document.createElement('div');
    icon.className = 'asdc-icon';
    icon.setAttribute('aria-hidden', 'true');
    icon.textContent = role === 'user' ? '🧑' : '🤖';

    const bubble = document.createElement('div');
    bubble.className = role === 'user' ? 'asdc-bubble asdc-bubble--me' : 'asdc-bubble asdc-bubble--bot';

    const contentWrap = document.createElement('div');
    contentWrap.className = 'asdc-bubble-content';

    const textDiv = document.createElement('div');
    textDiv.className = 'asdc-text';
    textDiv.innerHTML = stream ? '' : text;

    contentWrap.appendChild(textDiv);

    if (link) {
        const linkDiv = document.createElement('div');
        linkDiv.className = 'asdc-link';
        linkDiv.innerHTML = link;
        contentWrap.appendChild(linkDiv);
    }

    if (meta && (meta.duration || meta.scores)) {
        const metaDiv = document.createElement('div');
        metaDiv.className = 'asdc-meta';
        const parts = [];
        if (meta.duration) parts.push(`⏱️ ${Number(meta.duration).toFixed(2)}s`);
        if (meta.scores) {
            if (typeof meta.scores === 'object') {
                const vals = Object.values(meta.scores).map(v => Number(v).toFixed(2));
                parts.push(`🔍 ${vals.join(", ")}`);
            } else {
                parts.push(`🔍 ${Number(meta.scores).toFixed(2)}`);
            }
        }
        metaDiv.textContent = parts.join(' • ');
        contentWrap.appendChild(metaDiv);
    }

    // Controls row
    const controls = document.createElement('div');
    controls.className = 'asdc-controls';

    const copyBtn = document.createElement('button');
    copyBtn.type = 'button';
    copyBtn.className = 'asdc-icon-btn';
    copyBtn.title = 'Copy';
    copyBtn.textContent = '📋';
    copyBtn.onclick = () => {
        navigator.clipboard.writeText(text);
        copyBtn.textContent = '✅';
        setTimeout(() => copyBtn.textContent = '📋', 1500);
    };
    controls.appendChild(copyBtn);

    if (role === 'user') {
        const editBtn = document.createElement('button');
        editBtn.type = 'button';
        editBtn.className = 'asdc-icon-btn';
        editBtn.title = 'Edit & resend';
        editBtn.textContent = '✏️';
        editBtn.onclick = () => makeEditable(textDiv, text);
        controls.appendChild(editBtn);
    }

    contentWrap.appendChild(controls);
    bubble.appendChild(contentWrap);

    if (role === 'user') {
        row.appendChild(bubble);
        row.appendChild(icon);
    } else {
        row.appendChild(icon);
        row.appendChild(bubble);
    }

    chatBox.appendChild(row);
    requestAnimationFrame(() => scrollToBottom(chatBox));

    if (role === 'bot') {
        if (stream) {
            streamText(textDiv, text, 25, () => {
                appendRating(contentWrap);
                controls.style.display = 'flex';
                scrollToBottom(chatBox);
            }, () => scrollToBottom(chatBox));
        } else {
            appendRating(contentWrap);
        }
    }
}

function makeEditable(textDiv, originalText) {
    const wrapper = document.createElement('div');
    wrapper.className = 'asdc-edit-wrap';

    const input = document.createElement('input');
    input.type = 'text';
    input.value = originalText;
    input.className = 'asdc-edit-input';

    const btns = document.createElement('div');
    btns.className = 'asdc-edit-btns';

    const cancel = document.createElement('button');
    cancel.type = 'button';
    cancel.className = 'asdc-btn asdc-btn--gray';
    cancel.textContent = 'Cancel';
    cancel.onclick = () => {
        textDiv.textContent = originalText;
        wrapper.remove();
    };

    const send = document.createElement('button');
    send.type = 'button';
    send.className = 'asdc-btn asdc-btn--blue';
    send.textContent = 'Send';
    send.onclick = () => {
        const val = input.value.trim();
        if (val) {
            textDiv.textContent = val;
            wrapper.remove();
            sendQuery(val);
        }
    };

    btns.appendChild(cancel);
    btns.appendChild(send);
    wrapper.appendChild(input);
    wrapper.appendChild(btns);

    // replace current text with editor
    textDiv.textContent = '';
    textDiv.appendChild(wrapper);
    input.focus();
}

function showWelcome() {
    addMessage('bot', '👋 Welcome! How can I assist you today?');
}

function rateResponse(type) {
    const msg = document.createElement('div');
    msg.className = 'asdc-bubble asdc-bubble--note';
    msg.textContent = type === 'like' ? '👍 Thanks for liking the response!' : '👎 Thanks for your feedback!';
    chatBox.appendChild(msg);
    scrollToBottom(chatBox);

    fetch("http://127.0.0.1:8000/api/asdcfeedback/", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ feedback: type, query: lastUserQuery, collection_name: "chatbot_paw" })
    }).catch(() => { });
}

// --- Network ---
async function sendQuery(optionalQuery = null, silent = false) {
    if (isSending) return;

    const userQuery = (optionalQuery ?? inputEl.value).trim();
    if (!userQuery) return;

    isSending = true;
    lastUserQuery = userQuery;

    if (!silent) {
        addMessage('user', userQuery);
        inputEl.value = '';
        inputEl.disabled = true;
        sendBtn.disabled = true;
        loadingEl.hidden = false;
        chatBox.setAttribute('aria-busy', 'true');
    }

    const t0 = performance.now();
    try {
        const res = await fetch("http://127.0.0.1:8000/api/asdcask/", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ query: userQuery, collection_name: "chatbot_paw" })
        });
        const data = await res.json();
        const t1 = performance.now();
        const roundTrip = (t1 - t0) / 1000;

        if (!silent) {
            loadingEl.hidden = true;
            chatBox.setAttribute('aria-busy', 'false');
            addMessage('bot',
                data.answer || "Please ask a question related to the website.",
                { stream: true, link: data.link || "", meta: { duration: data.duration ?? roundTrip, scores: data.scores ?? null } }
            );
        }
    } catch {
        if (!silent) {
            loadingEl.hidden = true;
            chatBox.setAttribute('aria-busy', 'false');
            addMessage('bot', '❌ Error processing your request.');
        }
    } finally {
        isSending = false;
        if (!silent) {
            inputEl.disabled = false;
            sendBtn.disabled = false;
            inputEl.focus();
        }
    }
}
