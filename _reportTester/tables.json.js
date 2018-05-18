{
	"Patients": {"universalRelations":[{"child_cols":["PatientID"],"parent_cols":["PatientID"]}]}
	,"AllStaff": {"universalRelations":[{"child_cols":["StaffID"],"parent_cols":["StaffID"]}]}
	,"AllUsers": {"universalRelations":[{"child_cols":["InsertedBy"],"parent_cols":["UserID"]}]}
	,"AllUsers": {"universalRelations":[{"child_cols":["LastEditedBy"],"parent_cols":["UserID"]}]}
	,"Clinical_Data.PatientEncounter": {"universalRelations":[{"child_cols":["PatientEncounterID"],"parent_cols":["PatientEncounterID"]}]}
	,"InsurancePlan": { "universalRelations":[{ "child_cols": ["InsurancePlanID"], "parent_cols": ["InsurancePlanID"] }] }
	,"SlidingFeeScalePrograms": { "universalRelations":[{ "child_cols": ["SlidingFeeScaleProgramsID"], "parent_cols": ["SlidingFeeScaleProgramsID"] }] }
}
