const LoadingScreen = document.getElementById('LoadingScreen');
const WelcomeScreen = document.getElementById('WelcomeScreen');
const WelcomeTitle = document.getElementById('WelcomeTitle');
const LoginContainer = document.getElementById('LoginContainer');
const UpdateModal = document.getElementById('UpdateModal');
const BannedModal = document.getElementById('BannedModal');
const DonatorModal = document.getElementById('DonatorModal');
const UpdateButton = document.getElementById('UpdateButton');
const PasswordInput = document.getElementById('PasswordInput');
const PasswordToggle = document.getElementById('PasswordToggle');
const EyeIcon = document.getElementById('EyeIcon');
const LoginForm = document.getElementById('LoginForm');
const LoginBtn = document.getElementById('LoginBtn');

let bHasAutoLoggedIn = false;

const LoadingMessages = [
    { text: "Starting Up", subtext: "Getting everything ready and making sure you're all set..." },
    { text: "Checking For Updates", subtext: "Making sure you have the latest version and all updates are installed..." },
    { text: "Connecting", subtext: "Connecting to the servers and preparing your experience..." },
    { text: "Loading", subtext: "Almost there, just pulling everything together for you..." },
    { text: "Preparing", subtext: "Setting up your session and getting everything configured..." },
    { text: "Syncing", subtext: "Updating your data and making sure everything is current..." },
];

const RandomMessage = LoadingMessages[Math.floor(Math.random() * LoadingMessages.length)];
document.querySelector('.loading-text').textContent = RandomMessage.text;
document.querySelector('.loading-subtext').textContent = RandomMessage.subtext;

setTimeout(() => {
    LoadingScreen.classList.add('hidden');

    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({
            Action: 'CheckCredentials'
        });
    } else {
        LoginContainer.classList.add('show');
    }
}, 2000);

PasswordToggle.addEventListener('click', (Event) => {
    Event.preventDefault();
    Event.stopPropagation();

    const Type = PasswordInput.type === 'password' ? 'text' : 'password';
    PasswordInput.type = Type;

    PasswordInput.style.cssText = PasswordInput.style.cssText;

    if (Type === 'text') {
        EyeIcon.innerHTML = '<path d="M12 7C14.76 7 17 9.24 17 12C17 12.65 16.87 13.26 16.64 13.83L19.56 16.75C21.07 15.49 22.26 13.86 22.99 12C21.26 7.61 16.99 4.5 11.99 4.5C10.59 4.5 9.25 4.75 8.01 5.2L10.17 7.36C10.74 7.13 11.35 7 12 7ZM2 4.27L4.28 6.55L4.74 7.01C3.08 8.3 1.78 10.02 1 12C2.73 16.39 7 19.5 12 19.5C13.55 19.5 15.03 19.2 16.38 18.66L16.8 19.08L19.73 22L21 20.73L3.27 3L2 4.27ZM7.53 9.8L9.08 11.35C9.03 11.56 9 11.78 9 12C9 13.66 10.34 15 12 15C12.22 15 12.44 14.97 12.65 14.92L14.2 16.47C13.53 16.8 12.79 17 12 17C9.24 17 7 14.76 7 12C7 11.21 7.2 10.47 7.53 9.8ZM11.84 9.02L14.99 12.17L15.01 12.01C15.01 10.35 13.67 9.01 12.01 9.01L11.84 9.02Z" fill="white"/>';
    } else {
        EyeIcon.innerHTML = '<path d="M12 4.5C7 4.5 2.73 7.61 1 12C2.73 16.39 7 19.5 12 19.5C17 19.5 21.27 16.39 23 12C21.27 7.61 17 4.5 12 4.5ZM12 17C9.24 17 7 14.76 7 12C7 9.24 9.24 7 12 7C14.76 7 17 9.24 17 12C17 14.76 14.76 17 12 17ZM12 9C10.34 9 9 10.34 9 12C9 13.66 10.34 15 12 15C13.66 15 15 13.66 15 12C15 10.34 13.66 9 12 9Z" fill="white"/>';
    }
});

function ShowError(Title, Message) {
    const Container = document.getElementById('NotificationContainer');
    const ExistingNotifications = Container.querySelectorAll('.error-notification:not(.removing)');

    if (ExistingNotifications.length >= 5) {
        const OldestNotification = ExistingNotifications[ExistingNotifications.length - 1];
        RemoveNotification(OldestNotification);
    }

    const Notification = document.createElement('div');
    Notification.className = 'error-notification';

    const UrlRegex = /(https?:\/\/[^\s]+)/g;
    let MessageHtml = Message;
    if (UrlRegex.test(Message)) {
        MessageHtml = Message.replace(UrlRegex, '<a href="$1" target="_blank" style="color: #60a5fa; text-decoration: underline; font-weight: 600;">$1</a>');
    }

    Notification.innerHTML = `
        <div class="error-content">
            <svg class="error-icon" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M12 2C6.48 2 2 6.48 2 12C2 17.52 6.48 22 12 22C17.52 22 22 17.52 22 12C22 6.48 17.52 2 12 2ZM13 17H11V15H13V17ZM13 13H11V7H13V13Z" fill="white" />
            </svg>
            <div class="error-text">
                <div class="error-title">${Title}</div>
                <div class="error-message">${MessageHtml}</div>
            </div>
        </div>
    `;

    Container.insertBefore(Notification, Container.firstChild);

    requestAnimationFrame(() => {
        Notification.classList.add('show');
    });

    const AutoRemoveTimeout = setTimeout(() => {
        RemoveNotification(Notification);
    }, 5000);

    Notification.dataset.timeoutId = AutoRemoveTimeout;
}

function RemoveNotification(Notification) {
    if (Notification.classList.contains('removing')) return;

    Notification.classList.add('removing');

    if (Notification.dataset.timeoutId) {
        clearTimeout(parseInt(Notification.dataset.timeoutId));
    }

    Notification.classList.remove('show');
    Notification.classList.add('fade-out');

    setTimeout(() => {
        Notification.remove();
    }, 400);
}

function ShowBannedModal() {
    BannedModal.classList.add('show');
}

function ShowDonatorModal() {
    DonatorModal.classList.add('show');
}

function GetDominantColor(ImageUrl, Callback) {
    const Img = new Image();
    Img.crossOrigin = 'Anonymous';
    Img.src = ImageUrl;

    Img.onload = function () {
        const Canvas = document.createElement('canvas');
        const Context = Canvas.getContext('2d');
        Canvas.width = Img.width;
        Canvas.height = Img.height;
        Context.drawImage(Img, 0, 0);

        const ImageData = Context.getImageData(0, 0, Canvas.width, Canvas.height);
        const Data = ImageData.data;
        let R = 0, G = 0, B = 0, Count = 0;

        for (let i = 0; i < Data.length; i += 4) {
            if (Data[i + 3] > 128) {
                R += Data[i];
                G += Data[i + 1];
                B += Data[i + 2];
                Count++;
            }
        }

        R = Math.floor(R / Count);
        G = Math.floor(G / Count);
        B = Math.floor(B / Count);

        Callback(`rgb(${R}, ${G}, ${B})`);
    };
}

function ShowWelcomeScreen(Username, SkinUrl) {
    const WelcomeMessages = [
        {
            greeting: `Welcome Back, ${Username || 'Player'}!`,
            subtext: "Great to see you again. Loading your profile and getting everything ready..."
        },
        {
            greeting: `Hey ${Username || 'Player'}!`,
            subtext: "Good to have you back. Syncing your progress and setting things up..."
        },
        {
            greeting: `What's Up, ${Username || 'Player'}?`,
            subtext: "Glad you're here. Getting your account ready and loading your data..."
        },
        {
            greeting: `Welcome, ${Username || 'Player'}!`,
            subtext: "Happy to see you. Loading your profile and syncing everything..."
        },
        {
            greeting: `Let's Go, ${Username || 'Player'}!`,
            subtext: "Ready to get started. Setting up your session and loading your stats..."
        },
        {
            greeting: `Back Again, ${Username || 'Player'}?`,
            subtext: "Nice to see you. Loading everything up so you can jump right in..."
        }
    ];

    const RandomMessage = WelcomeMessages[Math.floor(Math.random() * WelcomeMessages.length)];

    WelcomeTitle.textContent = RandomMessage.greeting;
    document.querySelector('.welcome-subtitle').textContent = RandomMessage.subtext;

    const WelcomeLogo = document.getElementById('WelcomeLogo');
    if (WelcomeLogo) {
        if (SkinUrl) {
            GetDominantColor(SkinUrl, (DominantColor) => {
                WelcomeLogo.innerHTML = `
            <div class="glow-bg" style="background: ${DominantColor};"></div>
            <img src="${SkinUrl}" class="main-img" alt="Skin" style="width: 100%; height: 100%; object-fit: cover; border-color: ${DominantColor};">
        `;
            });
        } else {
            WelcomeLogo.innerHTML = `
            <img src="https://cdn.eonfn.dev/EonS17.png" alt="Eon Logo" style="width: 100%; height: 100%; object-fit: contain;">
        `;
        }
    }

    WelcomeScreen.classList.add('show');
    bHasAutoLoggedIn = true;

    setTimeout(() => {
        WelcomeScreen.classList.add('hidden');
    }, 2000);
}

function ShowUpdateRequired(DownloadUrl) {
    UpdateButton.href = DownloadUrl || '#';
    UpdateModal.classList.add('show');
}

LoginForm.addEventListener('submit', async (Event) => {
    Event.preventDefault();
    HandleLogin();
});

async function HandleLogin() {
    const Email = document.getElementById('EmailInput').value.trim();
    const Password = PasswordInput.value.trim();
    const bRememberMe = document.getElementById('RememberMe').checked;

    if (!Email || !Password) {
        ShowError('Access Denied', 'Please enter both email and password.');
        return;
    }

    if (!Email.includes('@')) {
        ShowError('Invalid Email', 'Please enter a valid email address.');
        return;
    }

    LoginBtn.disabled = true;
    LoginBtn.innerHTML = '<div class="spinner"></div>';

    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage({
            Action: 'Login',
            Email: Email,
            Password: Password,
            RememberMe: bRememberMe
        });
    } else {
        setTimeout(() => {
            ShowError('Connection Error', 'Unable to connect to the application.');
            LoginBtn.disabled = false;
            LoginBtn.textContent = 'Sign In';
        }, 1000);
    }
}

if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', HandleMessage);
}
window.addEventListener('message', HandleMessage);

function HandleLoginResponse(Data) {
    const ErrorMessages = {
        'Banned': 'You have been permanently banned from the servers.',
        'Deny': 'Access Denied.',
        'Invalid': 'Your email and/or password is invalid.',
        'Error': 'Unknown Error. Please try again.',
        'OUTDATED': `Your launcher version is outdated. Please update and download from ${Data.DownloadUrl}`,
        'Donator': 'Early access is currently available for donators only.'
    };

    if (Data.Status === 'Success') {
        LoginContainer.style.display = 'none';
        ShowWelcomeScreen(Data.Username, Data.SkinUrl);
        setTimeout(() => {
            WelcomeScreen.classList.add('hidden');
        }, 2000);
        return;
    }

    if (Data.Status === 'OUTDATED') {
        ShowUpdateRequired(Data.DownloadUrl);
        LoginBtn.disabled = false;
        LoginBtn.textContent = 'Sign In';
        return;
    }

    if (Data.Status === 'Banned') {
        ShowBannedModal();
        LoginBtn.disabled = false;
        LoginBtn.textContent = 'Sign In';
        return;
    }

    if (Data.Status === 'Donator') {
        ShowDonatorModal();
        LoginBtn.disabled = false;
        LoginBtn.textContent = 'Sign In';
        return;
    }

    const ErrorMessage = ErrorMessages[Data.Status] || 'Login failed. Please try again.';
    ShowError('Login Failed', ErrorMessage);
    LoginBtn.disabled = false;
    LoginBtn.textContent = 'Sign In';
}

function HandleMessage(Event) {
    const Data = Event.data;

    if (Data.Action === 'AutoLogin') {
        if (Data.Status === 'Success') {
            ShowWelcomeScreen(Data.Username, Data.SkinUrl);
            return;
        }

        if (Data.Status === 'OUTDATED') {
            ShowUpdateRequired(Data.DownloadUrl);
            return;
        }

        LoginContainer.classList.add('show');
        return;
    }

    if (Data.Action === 'ShowLogin') {
        LoginContainer.classList.add('show');
        return;
    }

    if (Data.Action === 'LoginResponse') {
        HandleLoginResponse(Data);
        return;
    }
}

document.getElementById('EmailInput').addEventListener('keypress', (Event) => {
    if (Event.key === 'Enter') {
        Event.preventDefault();
        HandleLogin();
    }
});