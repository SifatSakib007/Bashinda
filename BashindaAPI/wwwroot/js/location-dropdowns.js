// Location dropdowns functionality
$(document).ready(function () {
    // Initialize dropdowns
    loadDivisions();

    // Division change event
    $('#division-dropdown').on('change', function () {
        var divisionId = $(this).val();
        if (divisionId) {
            loadDistricts(divisionId);
            // Reset dependent dropdowns
            resetDropdown('#upazila-dropdown');
            resetDropdown('#ward-dropdown');
            resetDropdown('#village-dropdown');

            // Disable area type radios until upazila is selected
            $('input[name="AreaType"]').prop('disabled', true);
        } else {
            resetDropdown('#district-dropdown');
            resetDropdown('#upazila-dropdown');
            resetDropdown('#ward-dropdown');
            resetDropdown('#village-dropdown');
        }
    });

    // District change event
    $('#district-dropdown').on('change', function () {
        var districtId = $(this).val();
        if (districtId) {
            loadUpazilas(districtId);
            // Reset dependent dropdowns
            resetDropdown('#ward-dropdown');
            resetDropdown('#village-dropdown');
        } else {
            resetDropdown('#upazila-dropdown');
            resetDropdown('#ward-dropdown');
            resetDropdown('#village-dropdown');
        }
    });

    // Upazila change event
    $('#upazila-dropdown').on('change', function () {
        var upazilaId = $(this).val();
        if (upazilaId) {
            // Enable area type selection after upazila is selected
            $('input[name="AreaType"]').prop('disabled', false);
        } else {
            // Disable area type selection if no upazila selected
            $('input[name="AreaType"]').prop('disabled', true);
            $('input[name="AreaType"]').prop('checked', false);
        }
        
        // Reset dependent dropdowns
        resetDropdown('#ward-dropdown');
        resetDropdown('#village-dropdown');
    });

    // Area type change event
    $('input[name="AreaType"]').on('change', function () {
        var areaType = $(this).val();
        var upazilaId = $('#upazila-dropdown').val();
        
        if (upazilaId && areaType !== undefined) {
            loadWards(upazilaId, areaType);
            // Reset dependent dropdown
            resetDropdown('#village-dropdown');
        } else {
            resetDropdown('#ward-dropdown');
            resetDropdown('#village-dropdown');
        }
    });

    // Ward change event
    $('#ward-dropdown').on('change', function () {
        var wardId = $(this).val();
        if (wardId) {
            loadVillages(wardId);
        } else {
            resetDropdown('#village-dropdown');
        }
    });

    // Function to load divisions
    function loadDivisions() {
        $.ajax({
            url: '/api/Locations/divisions',
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var dropdown = $('#division-dropdown');
                dropdown.empty();
                dropdown.append('<option value="">-- Select Division --</option>');
                
                $.each(data, function (i, division) {
                    dropdown.append($('<option></option>').attr('value', division.id).text(division.name));
                });
                
                // If editing and division is already selected
                var selectedDivision = $('#selected-division-id').val();
                if (selectedDivision && selectedDivision !== '0') {
                    dropdown.val(selectedDivision);
                    loadDistricts(selectedDivision);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error loading divisions: ' + error);
            }
        });
    }

    // Function to load districts based on division
    function loadDistricts(divisionId) {
        $.ajax({
            url: '/api/Locations/districts/' + divisionId,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var dropdown = $('#district-dropdown');
                dropdown.empty();
                dropdown.prop('disabled', false);
                dropdown.append('<option value="">-- Select District --</option>');
                
                $.each(data, function (i, district) {
                    dropdown.append($('<option></option>').attr('value', district.id).text(district.name));
                });
                
                // If editing and district is already selected
                var selectedDistrict = $('#selected-district-id').val();
                if (selectedDistrict && selectedDistrict !== '0') {
                    dropdown.val(selectedDistrict);
                    loadUpazilas(selectedDistrict);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error loading districts: ' + error);
            }
        });
    }

    // Function to load upazilas based on district
    function loadUpazilas(districtId) {
        $.ajax({
            url: '/api/Locations/upazilas/' + districtId,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var dropdown = $('#upazila-dropdown');
                dropdown.empty();
                dropdown.prop('disabled', false);
                dropdown.append('<option value="">-- Select Upazila --</option>');
                
                $.each(data, function (i, upazila) {
                    dropdown.append($('<option></option>').attr('value', upazila.id).text(upazila.name));
                });
                
                // If editing and upazila is already selected
                var selectedUpazila = $('#selected-upazila-id').val();
                if (selectedUpazila && selectedUpazila !== '0') {
                    dropdown.val(selectedUpazila);
                    
                    // Enable area type selection
                    $('input[name="AreaType"]').prop('disabled', false);
                    
                    // If area type is already selected
                    var selectedAreaType = $('#selected-area-type').val();
                    if (selectedAreaType && selectedAreaType !== '-1') {
                        $('input[name="AreaType"][value="' + selectedAreaType + '"]').prop('checked', true);
                        loadWards(selectedUpazila, selectedAreaType);
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error('Error loading upazilas: ' + error);
            }
        });
    }

    // Function to load wards based on upazila and area type
    function loadWards(upazilaId, areaType) {
        $.ajax({
            url: '/api/Locations/wards/' + upazilaId + '/' + areaType,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var dropdown = $('#ward-dropdown');
                dropdown.empty();
                dropdown.prop('disabled', false);
                dropdown.append('<option value="">-- Select Ward --</option>');
                
                $.each(data, function (i, ward) {
                    dropdown.append($('<option></option>').attr('value', ward.id).text(ward.name));
                });
                
                // If editing and ward is already selected
                var selectedWard = $('#selected-ward-id').val();
                if (selectedWard && selectedWard !== '0') {
                    dropdown.val(selectedWard);
                    loadVillages(selectedWard);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error loading wards: ' + error);
            }
        });
    }

    // Function to load villages based on ward
    function loadVillages(wardId) {
        $.ajax({
            url: '/api/Locations/villages/' + wardId,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                var dropdown = $('#village-dropdown');
                dropdown.empty();
                dropdown.prop('disabled', false);
                dropdown.append('<option value="">-- Select Village/Area --</option>');
                
                $.each(data, function (i, village) {
                    dropdown.append($('<option></option>').attr('value', village.id).text(village.name));
                });
                
                // If editing and village is already selected
                var selectedVillage = $('#selected-village-id').val();
                if (selectedVillage && selectedVillage !== '0') {
                    dropdown.val(selectedVillage);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error loading villages: ' + error);
            }
        });
    }

    // Helper function to reset a dropdown
    function resetDropdown(selector) {
        var dropdown = $(selector);
        dropdown.empty();
        dropdown.prop('disabled', true);
        dropdown.append('<option value="">-- Select --</option>');
    }
}); 