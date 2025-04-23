console.log("Accessibility.js has been loaded and is running!");

//Functionality to automatically save user preferences when they log out/in
document.addEventListener("DOMContentLoaded", function () {
    console.log("✅ Applying preferences from window.userPreferences");

    const prefs = window.userPreferences || {};

    console.log("🧪 Loaded prefs:", prefs);

    if (!prefs.fontSize && !prefs.contrastMode && !prefs.textToSpeech) {
        console.warn("⚠️ No preferences detected, skipping style updates");
    }

    // Font Size
    const fontSizeDropdown = document.getElementById("fontSize");
    switch (prefs.fontSize) {
        case "small":
            document.body.style.fontSize = "14px";
            if (fontSizeDropdown) fontSizeDropdown.value = "small";
            break;
        case "large":
            document.body.style.fontSize = "18px";
            if (fontSizeDropdown) fontSizeDropdown.value = "large";
            break;
        case "xlarge":
            document.body.style.fontSize = "22px";
            if (fontSizeDropdown) fontSizeDropdown.value = "xlarge";
            break;
        default:
            document.body.style.fontSize = "16px";
            if (fontSizeDropdown) fontSizeDropdown.value = "medium";
            break;
    }

    // Contrast
    if (prefs.contrastMode === true) {
        document.body.classList.add("high-contrast");
    }

    // Optional: Text-to-Speech setup if needed
    if (prefs.textToSpeech === true) {
        console.log("🗣️ Text-to-Speech is marked as enabled");
        // You could pre-check a box, toggle a UI element, etc.
    }
});

document.addEventListener("DOMContentLoaded", function() {
    // Font Size Adjustment
    const fontSizeDropdown = document.getElementById("fontSize");
    if (fontSizeDropdown) {
        fontSizeDropdown.addEventListener("change", function() {
            document.body.style.fontSize = this.value === "small" ? "14px" :
                                           this.value === "medium" ? "16px" :
                                           this.value === "large" ? "18px" :
                                           this.value === "xlarge" ? "22px" : "16px";
        });
    }

    // High Contrast Mode Toggle
    const contrastButton = document.getElementById("contrastToggle");
    if (contrastButton) {
        contrastButton.addEventListener("click", function() {
            document.body.classList.toggle("high-contrast");
        });
    }

    // Text-to-Speech (Toggleable)
    let speech = new SpeechSynthesisUtterance();
    let isSpeaking = false;

    const speechButton = document.getElementById("speechToggle");
    if (speechButton) {
        speechButton.addEventListener("click", function() {
            if (!isSpeaking) {
                speech.text = document.body.innerText;
                speech.lang = "en-US";
                speech.rate = 1;
                window.speechSynthesis.speak(speech);
                isSpeaking = true;
                speechButton.textContent = "Stop Text-to-Speech";
            } else {
                window.speechSynthesis.cancel();
                isSpeaking = false;
                speechButton.textContent = "Enable Text-to-Speech";
            }
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    console.log("🚀 accessibility.js has loaded!");

    const fontSizeDropdown = document.getElementById("fontSize");
    if (fontSizeDropdown) {
        console.log("✅ Font size dropdown found.");
        fontSizeDropdown.addEventListener("change", function () {
            console.log("📝 Font size changed!");
            savePreferences();
        });
    } else {
        console.warn("⚠️ Font size dropdown not found!");
    }

    const contrastButton = document.getElementById("contrastToggle");
    if (contrastButton) {
        console.log("✅ Contrast button found.");
        contrastButton.addEventListener("click", function () {
            console.log("🔲 Contrast mode toggled!");
            savePreferences();
        });
    } else {
        console.warn("⚠️ Contrast button not found!");
    }

    const speechButton = document.getElementById("speechToggle");
    if (speechButton) {
        console.log("✅ Speech button found.");
        speechButton.addEventListener("click", function () {
            console.log("🗣️ Text-to-Speech toggled!");
            savePreferences();
        });
    } else {
        console.warn("⚠️ Speech button not found!");
    }
});

window.savePreferences = async function () {
    console.log("✅ savePreferences() is now globally available!");
    const fontSize = document.getElementById("fontSize")?.value || "default";
    const contrastMode = document.body.classList.contains("high-contrast");
    const textToSpeech = window.isSpeaking || false;

    console.log("✅ Preparing to send API request...");
    console.log("📝 Data to be sent:", { fontSize, contrastMode, textToSpeech });

    try {
        console.log("📡 About to make fetch() request...");
        const response = await fetch("/api/user/preferences", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ fontSize, contrastMode, textToSpeech }),
        });

        console.log("📡 Fetch request has been made... waiting for response...");

        if (!response.ok) {
            console.error("❌ Failed to save preferences. Status:", response.status);
        } else {
            console.log("✅ Preferences successfully saved!");
        }
    } catch (error) {
        console.error("❌ Error during fetch():", error);
    }
};

console.log("✅ accessibility.js has fully executed.");
