// Renter profile form functionality
$(document).ready(function() {
    // Function to toggle adult/child specific fields
    function toggleAdultChildFields() {
        var isAdult = $('#IsAdult').is(':checked');
        
        if (isAdult) {
            $('.adult-fields').show();
            $('.child-fields').hide();
            $('#NationalId').prop('required', true);
            $('#BirthRegistrationNo').prop('required', false);
        } else {
            $('.adult-fields').hide();
            $('.child-fields').show();
            $('#NationalId').prop('required', false);
            $('#BirthRegistrationNo').prop('required', true);
        }
    }
    
    // Initial toggle based on current value
    toggleAdultChildFields();
    
    // Toggle on change
    $('#IsAdult').change(function() {
        toggleAdultChildFields();
    });

    // Form submission handling
    $('#renter-profile-form').submit(function(e) {
        e.preventDefault();
        
        // Validate form
        if (!this.checkValidity()) {
            e.stopPropagation();
            $(this).addClass('was-validated');
            return;
        }
        
        // Create FormData object
        var formData = new FormData(this);
        
        // Add files if present
        var selfImageInput = $('#self-image')[0];
        if (selfImageInput && selfImageInput.files.length > 0) {
            formData.append('SelfImage', selfImageInput.files[0]);
        }
        
        var isAdult = $('#IsAdult').is(':checked');
        if (isAdult) {
            var nationalIdImageInput = $('#nationalid-image')[0];
            if (nationalIdImageInput && nationalIdImageInput.files.length > 0) {
                formData.append('NationalIdImage', nationalIdImageInput.files[0]);
            }
        } else {
            var birthRegImageInput = $('#birthregistration-image')[0];
            if (birthRegImageInput && birthRegImageInput.files.length > 0) {
                formData.append('BirthRegistrationImage', birthRegImageInput.files[0]);
            }
        }
        
        // Get the form action URL
        var formAction = $(this).attr('action');
        
        // Submit form via AJAX
        $.ajax({
            url: formAction,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function(response) {
                // Show success message
                toastr.success('Profile updated successfully');
                
                // Redirect to profile page after a short delay
                setTimeout(function() {
                    window.location.href = '/RenterProfiles/MyProfile';
                }, 1500);
            },
            error: function(xhr, status, error) {
                // Show error message
                toastr.error('An error occurred while saving your profile');
                
                // Display validation errors if any
                if (xhr.responseJSON) {
                    var errors = xhr.responseJSON.errors;
                    for (var key in errors) {
                        var errorMsg = errors[key][0];
                        toastr.error(errorMsg);
                        
                        // Add validation styling to the field
                        $('[name="' + key + '"]').addClass('is-invalid');
                        $('[data-valmsg-for="' + key + '"]').text(errorMsg);
                    }
                }
            }
        });
    });

    // Custom file input label update
    $('.custom-file-input').on('change', function() {
        var fileName = $(this).val().split('\\').pop();
        $(this).siblings('.custom-file-label').addClass('selected').html(fileName);
    });
}); 