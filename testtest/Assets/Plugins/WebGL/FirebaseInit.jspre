// Firebase SDK загружается из CDN при старте WebGL-билда
// Конфиг подставляется из FirebaseManager.cs через index.html
(function() {
  var scripts = [
    'https://www.gstatic.com/firebasejs/9.23.0/firebase-app-compat.js',
    'https://www.gstatic.com/firebasejs/9.23.0/firebase-auth-compat.js',
    'https://www.gstatic.com/firebasejs/9.23.0/firebase-firestore-compat.js'
  ];

  var loaded = 0;
  scripts.forEach(function(src) {
    var s = document.createElement('script');
    s.src = src;
    s.onload = function() {
      loaded++;
      if (loaded === scripts.length) {
        firebase.initializeApp({
          apiKey: "AIzaSyD2PW7-21-pkiW3q9L2ZFrfmsIrluQcNh4",
          authDomain: "history-game-59b70.firebaseapp.com",
          projectId: "history-game-59b70"
        });
        console.log('[Firebase] Initialized for WebGL');
      }
    };
    document.head.appendChild(s);
  });
})();
