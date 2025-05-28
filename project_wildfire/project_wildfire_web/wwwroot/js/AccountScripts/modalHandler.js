import { initSavedLocations } from '../ProfilePartial/savedLocations.js';
import { getUserId } from '../site.js';

// Get user ID from site.js
const userId = await getUserId();

// If userId is defined, initialize profile modal
if (userId) {
    // Detect profile element and initialize Bootstrap modal
    const modalElement = document.getElementById('profileModal');
    const profileModal = new bootstrap.Modal(modalElement);
    if (!modalElement || !profileModal) {
        console.error('User profile not found.');
    } else {

        // Callback function when modal displays
        const handleModalShown = () => {
            try {
                // TODO: Add controller calls to update the modal content
                initSavedLocations();
            } catch (error) {
                console.error('Error initializing location editing:', error);
            }
        };
        
        // Attach event listener for when the modal is shown
        modalElement.addEventListener('shown.bs.modal', handleModalShown);
        
        // Handle clicks on the profile link
        document.querySelectorAll('#manage')?.forEach(link => {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                profileModal.show();
                console.log('Profile link clicked');
            });
        });
    }
}