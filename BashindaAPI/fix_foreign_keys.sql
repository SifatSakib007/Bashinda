-- Drop existing foreign key constraints
ALTER TABLE [ApartmentOwnerProfiles] DROP CONSTRAINT IF EXISTS [FK_ApartmentOwnerProfiles_Districts_DistrictId];
ALTER TABLE [ApartmentOwnerProfiles] DROP CONSTRAINT IF EXISTS [FK_ApartmentOwnerProfiles_Divisions_DivisionId];
ALTER TABLE [ApartmentOwnerProfiles] DROP CONSTRAINT IF EXISTS [FK_ApartmentOwnerProfiles_Upazilas_UpazilaId];
ALTER TABLE [ApartmentOwnerProfiles] DROP CONSTRAINT IF EXISTS [FK_ApartmentOwnerProfiles_Wards_WardId];
ALTER TABLE [ApartmentOwnerProfiles] DROP CONSTRAINT IF EXISTS [FK_ApartmentOwnerProfiles_Villages_VillageId];

ALTER TABLE [RenterProfiles] DROP CONSTRAINT IF EXISTS [FK_RenterProfiles_Districts_DistrictId];
ALTER TABLE [RenterProfiles] DROP CONSTRAINT IF EXISTS [FK_RenterProfiles_Divisions_DivisionId];
ALTER TABLE [RenterProfiles] DROP CONSTRAINT IF EXISTS [FK_RenterProfiles_Upazilas_UpazilaId];
ALTER TABLE [RenterProfiles] DROP CONSTRAINT IF EXISTS [FK_RenterProfiles_Wards_WardId];
ALTER TABLE [RenterProfiles] DROP CONSTRAINT IF EXISTS [FK_RenterProfiles_Villages_VillageId];

-- Recreate foreign key constraints with NO ACTION delete behavior
ALTER TABLE [ApartmentOwnerProfiles] ADD CONSTRAINT [FK_ApartmentOwnerProfiles_Districts_DistrictId] 
    FOREIGN KEY ([DistrictId]) REFERENCES [Districts] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [ApartmentOwnerProfiles] ADD CONSTRAINT [FK_ApartmentOwnerProfiles_Divisions_DivisionId] 
    FOREIGN KEY ([DivisionId]) REFERENCES [Divisions] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [ApartmentOwnerProfiles] ADD CONSTRAINT [FK_ApartmentOwnerProfiles_Upazilas_UpazilaId] 
    FOREIGN KEY ([UpazilaId]) REFERENCES [Upazilas] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [ApartmentOwnerProfiles] ADD CONSTRAINT [FK_ApartmentOwnerProfiles_Wards_WardId] 
    FOREIGN KEY ([WardId]) REFERENCES [Wards] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [ApartmentOwnerProfiles] ADD CONSTRAINT [FK_ApartmentOwnerProfiles_Villages_VillageId] 
    FOREIGN KEY ([VillageId]) REFERENCES [Villages] ([Id]) ON DELETE NO ACTION;

ALTER TABLE [RenterProfiles] ADD CONSTRAINT [FK_RenterProfiles_Districts_DistrictId] 
    FOREIGN KEY ([DistrictId]) REFERENCES [Districts] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [RenterProfiles] ADD CONSTRAINT [FK_RenterProfiles_Divisions_DivisionId] 
    FOREIGN KEY ([DivisionId]) REFERENCES [Divisions] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [RenterProfiles] ADD CONSTRAINT [FK_RenterProfiles_Upazilas_UpazilaId] 
    FOREIGN KEY ([UpazilaId]) REFERENCES [Upazilas] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [RenterProfiles] ADD CONSTRAINT [FK_RenterProfiles_Wards_WardId] 
    FOREIGN KEY ([WardId]) REFERENCES [Wards] ([Id]) ON DELETE NO ACTION;
ALTER TABLE [RenterProfiles] ADD CONSTRAINT [FK_RenterProfiles_Villages_VillageId] 
    FOREIGN KEY ([VillageId]) REFERENCES [Villages] ([Id]) ON DELETE NO ACTION; 