{
	  "Asset": { "universalRelations": [{ "parent_cols": ["id"], "child_cols": ["assetid"] }] }
	, "AssetItem": { "universalRelations": [{ "parent_cols": ["id"], "child_cols": ["assetitemid"] }] }
	, "Truck": { "universalRelations": [{ "parent_cols": ["id"], "child_cols": ["truckid"] }] }
	, "PackList": { "universalRelations": [{ "parent_cols": ["id"], "child_cols": ["packListid"] }] }
	, "Inventory": { "universalRelations": [{ "parent_cols": ["id"], "child_cols": ["Inventoryid"] }] }
	, "Delivery": { "universalRelations": [{ "parent_cols": ["id"], "child_cols": ["Deliveryid"] }] }
	, "BOEBallotTracking.dbo.LK_ELECTION": { "universalRelations": [{ "parent_cols": ["ID"], "child_cols": ["electionid"] }] }
	, "BOEBallotTracking.dbo.POLLING_PLACE": { "universalRelations": [{ "parent_cols": ["polling_place_id"], "child_cols": ["pollingPlaceid"] }] }
	, "BOEBallotTracking.dbo.EPB_SITE_INFO": { "universalRelations": [{ "parent_cols": ["name_abbr"], "child_cols": ["site_lbl"] }] }
	, "BOEBallotTracking.dbo.EPB_SITE_INFO": { "universalRelations": [{ "parent_cols": ["id"], "child_cols": ["OneStopID"] }] }
}
