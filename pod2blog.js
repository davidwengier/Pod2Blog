// Speech Recognition API for Pod2Blog
window.Pod2Blog = {
    recognition: null,
    pauseDetectionTimer: null,
    pauseDetectionEnabled: false,
    pauseDetectionSeconds: 3,
    dotNetHelper: null,
    
    initializeSpeechRecognition: function(dotNetHelper) {
        if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
            console.error('Speech recognition not supported');
            return false;
        }

        this.dotNetHelper = dotNetHelper;
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        this.recognition = new SpeechRecognition();
        this.recognition.continuous = true;
        this.recognition.interimResults = true;
        this.recognition.lang = 'en-US';

        let finalTranscript = '';
        const self = this;

        this.recognition.onresult = function(event) {
            let interimTranscript = '';
            
            for (let i = event.resultIndex; i < event.results.length; i++) {
                const transcript = event.results[i][0].transcript;
                if (event.results[i].isFinal) {
                    finalTranscript += transcript + ' ';
                } else {
                    interimTranscript += transcript;
                }
            }

            dotNetHelper.invokeMethodAsync('OnTranscriptUpdate', finalTranscript + interimTranscript);
            
            // Reset pause detection timer on new speech
            if (self.pauseDetectionEnabled) {
                self.resetPauseDetectionTimer(finalTranscript);
            }
        };

        this.recognition.onerror = function(event) {
            console.error('Speech recognition error:', event.error);
            dotNetHelper.invokeMethodAsync('OnRecognitionError', event.error);
        };

        this.recognition.onend = function() {
            // Clear any pause detection timer
            if (self.pauseDetectionTimer) {
                clearTimeout(self.pauseDetectionTimer);
                self.pauseDetectionTimer = null;
            }
            dotNetHelper.invokeMethodAsync('OnRecognitionEnd', finalTranscript);
            finalTranscript = '';
        };

        return true;
    },
    
    resetPauseDetectionTimer: function(currentTranscript) {
        const self = this;
        
        // Clear existing timer
        if (this.pauseDetectionTimer) {
            clearTimeout(this.pauseDetectionTimer);
        }
        
        // Only set timer if we have some speech
        if (currentTranscript.trim().length > 0) {
            console.log(`Pause detection: Waiting ${this.pauseDetectionSeconds} seconds...`);
            this.pauseDetectionTimer = setTimeout(() => {
                console.log('Pause detected - auto-stopping recording');
                self.stopRecording();
            }, this.pauseDetectionSeconds * 1000);
        }
    },

    startRecording: function(enablePauseDetection = false, pauseSeconds = 3) {
        if (this.recognition) {
            this.pauseDetectionEnabled = enablePauseDetection;
            this.pauseDetectionSeconds = pauseSeconds;
            console.log(`Starting recording with pause detection: ${enablePauseDetection} (${pauseSeconds}s)`);
            this.recognition.start();
            return true;
        }
        return false;
    },

    stopRecording: function() {
        if (this.recognition) {
            // Clear pause detection timer
            if (this.pauseDetectionTimer) {
                clearTimeout(this.pauseDetectionTimer);
                this.pauseDetectionTimer = null;
            }
            this.recognition.stop();
            return true;
        }
        return false;
    },

    copyToClipboard: async function(text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (err) {
            console.error('Failed to copy:', err);
            return false;
        }
    },

    downloadFile: function(filename, content) {
        const blob = new Blob([content], { type: 'text/markdown' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }
};

// Utility function to scroll element to bottom
window.scrollToBottom = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

// Text-to-speech function that returns a promise
window.speakText = function(text, voiceName = '', rate = 1.0, pitch = 1.0) {
    return new Promise((resolve, reject) => {
        try {
            // Cancel any ongoing speech
            window.speechSynthesis.cancel();
            
            const utterance = new SpeechSynthesisUtterance(text);
            utterance.rate = rate; // Speed: 0.5 to 2.0
            utterance.pitch = pitch; // Pitch: 0.0 to 2.0
            utterance.volume = 1.0; // Full volume
            
            // Set up event handlers
            utterance.onend = () => {
                console.log('Speech finished');
                resolve(true);
            };
            
            utterance.onerror = (event) => {
                console.error('Speech error:', event);
                reject(event.error);
            };
            
            let hasSpoken = false; // Prevent multiple invocations
            
            // Wait for voices to load before trying to set one
            const setVoiceAndSpeak = () => {
                if (hasSpoken) return; // Already started speaking
                hasSpoken = true;
                
                const voices = window.speechSynthesis.getVoices();
                console.log('Available voices:', voices.length);
                
                if (voices.length > 0) {
                    let selectedVoice = null;
                    
                    // If a specific voice name was requested, try to find it
                    if (voiceName) {
                        selectedVoice = voices.find(v => v.name === voiceName);
                        console.log('Looking for voice:', voiceName, 'Found:', !!selectedVoice);
                    }
                    
                    // Fall back to a good default English voice
                    if (!selectedVoice) {
                        selectedVoice = voices.find(v => v.lang.startsWith('en') && (v.name.includes('Natural') || v.name.includes('Microsoft'))) 
                                     || voices.find(v => v.lang.startsWith('en'));
                    }
                    
                    if (selectedVoice) {
                        utterance.voice = selectedVoice;
                        console.log('Using voice:', selectedVoice.name);
                    }
                }
                
                console.log('Speaking text with rate:', rate, 'pitch:', pitch);
                window.speechSynthesis.speak(utterance);
            };
            
            // If voices are already loaded, use them immediately
            if (window.speechSynthesis.getVoices().length > 0) {
                setVoiceAndSpeak();
            } else {
                // Otherwise wait for them to load
                window.speechSynthesis.onvoiceschanged = setVoiceAndSpeak;
                // Also try after a short delay as fallback
                setTimeout(setVoiceAndSpeak, 100);
            }
        } catch (err) {
            console.error('Failed to speak text:', err);
            reject(err);
        }
    });
};

// Get available voices
window.getVoices = function() {
    return new Promise((resolve) => {
        const voices = window.speechSynthesis.getVoices();
        if (voices.length > 0) {
            resolve(voices);
        } else {
            window.speechSynthesis.onvoiceschanged = () => {
                resolve(window.speechSynthesis.getVoices());
            };
            // Fallback after timeout
            setTimeout(() => {
                resolve(window.speechSynthesis.getVoices());
            }, 100);
        }
    });
};

window.stopSpeaking = function() {
    try {
        window.speechSynthesis.cancel();
        return true;
    } catch (err) {
        console.error('Failed to stop speaking:', err);
        return false;
    }
};

// Global array to track active audio elements
window.activeAudioElements = [];

// Play audio from byte array
window.playAudioData = function(audioData) {
    return new Promise((resolve, reject) => {
        try {
            // Convert byte array to blob
            const blob = new Blob([new Uint8Array(audioData)], { type: 'audio/mpeg' });
            const url = URL.createObjectURL(blob);
            
            // Create audio element
            const audio = new Audio(url);
            
            // Track this audio element
            window.activeAudioElements.push(audio);
            
            audio.onended = () => {
                URL.revokeObjectURL(url);
                // Remove from tracking
                const index = window.activeAudioElements.indexOf(audio);
                if (index > -1) {
                    window.activeAudioElements.splice(index, 1);
                }
                console.log('AI audio playback finished');
                resolve(true);
            };
            
            audio.onerror = (error) => {
                URL.revokeObjectURL(url);
                // Remove from tracking
                const index = window.activeAudioElements.indexOf(audio);
                if (index > -1) {
                    window.activeAudioElements.splice(index, 1);
                }
                console.error('AI audio playback error:', error);
                reject(error);
            };
            
            // Play audio
            audio.play().catch(error => {
                URL.revokeObjectURL(url);
                // Remove from tracking
                const index = window.activeAudioElements.indexOf(audio);
                if (index > -1) {
                    window.activeAudioElements.splice(index, 1);
                }
                console.error('Failed to play AI audio:', error);
                reject(error);
            });
        } catch (err) {
            console.error('Failed to process audio data:', err);
            reject(err);
        }
    });
};

// Stop all audio playback
window.stopAllAudio = function() {
    try {
        // Stop browser TTS
        window.speechSynthesis.cancel();
        
        // Stop all tracked audio elements
        window.activeAudioElements.forEach(audio => {
            try {
                audio.pause();
                audio.currentTime = 0;
            } catch (err) {
                console.error('Failed to stop audio element:', err);
            }
        });
        
        // Clear the array
        window.activeAudioElements = [];
        
        console.log('All audio stopped');
        return true;
    } catch (err) {
        console.error('Failed to stop all audio:', err);
        return false;
    }
};

// Global helper functions for clipboard and file download
window.copyToClipboard = async function(text) {
    try {
        await navigator.clipboard.writeText(text);
        alert('✅ Copied to clipboard!');
        return true;
    } catch (err) {
        console.error('Failed to copy:', err);
        alert('❌ Failed to copy to clipboard');
        return false;
    }
};

window.downloadTextFile = function(filename, content) {
    try {
        const blob = new Blob([content], { type: 'text/plain' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
        return true;
    } catch (err) {
        console.error('Failed to download:', err);
        alert('❌ Failed to download file');
        return false;
    }
};
