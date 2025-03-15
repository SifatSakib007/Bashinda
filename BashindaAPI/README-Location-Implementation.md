# Bangladesh Location Implementation

This document outlines the changes made to implement the Bangladesh-specific location structure in the Bashinda application.

## Database Structure

We've created a hierarchical location structure with the following entities:

1. **Division** (Top level: Dhaka, Khulna, etc.)
2. **District** (Second level: depends on Division)
3. **Upazila** (Third level: depends on District)
4. **Ward** (Fourth level: depends on Upazila and AreaType)
5. **Village/Area** (Fifth level: depends on Ward)

The AreaType enum defines the type of area:
- CityCorporation
- Union
- Pourasava

## Model Changes

### Location Models
- Created `Division`, `District`, `Upazila`, `Ward`, and `Village` models
- Added navigation properties to establish relationships between models
- Added `AreaType` enum for the type of area

### Profile Models
- Updated `RenterProfile` and `ApartmentOwnerProfile` models to include location fields
- Added foreign keys for `DivisionId`, `DistrictId`, `UpazilaId`, `WardId`, and `VillageId`
- Added `AreaType`, `PostCode`, and `HoldingNo` fields
- Added navigation properties to the location models

## DTO Changes

### Location DTOs
- Created DTOs for each location entity
- Created DTOs for creating new location records
- Created an `AddressDto` for use in profile responses

### Profile DTOs
- Updated `RenterProfileDto` and `ApartmentOwnerProfileDto` to include location information
- Updated `CreateRenterProfileDto` and `CreateApartmentOwnerProfileDto` to include location fields
- Updated `UpdateRenterProfileDto` and `UpdateApartmentOwnerProfileDto` to include location fields
- Updated `RenterProfileListDto` and `ApartmentOwnerProfileListDto` to include basic location information

## Controller Changes

### New LocationsController
- Created a new controller for handling location data
- Added endpoints for retrieving divisions, districts, upazilas, wards, and villages
- Added a seed data endpoint for populating the location tables with sample data

### Profile Controllers
- Updated `RenterProfilesController` and `ApartmentOwnerProfilesController` to handle the new location fields
- Added validation for location data in create and update methods
- Updated response DTOs to include location information

## Next Steps

1. Run the migration to update the database schema
2. Seed the location data using the `/api/Locations/SeedData` endpoint
3. Update the frontend to use the new location endpoints for cascading dropdowns
4. Test the profile creation and update functionality with the new location fields

## Sample Location Data

The seed data includes:
- 5 divisions (Dhaka, Chittagong, Khulna, Rajshahi, Sylhet)
- 5 districts for Dhaka division
- 5 upazilas for Dhaka district
- 5 wards for Dhaka City upazila (City Corporation)
- 5 villages/areas for Ward 1

Additional data should be added for other divisions, districts, etc. as needed. 