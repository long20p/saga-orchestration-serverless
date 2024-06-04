#this is a file with recommended variable names
prefix          = "lp"
environment     = "dev"
common_name		= "sagalogic"
location        = "westeurope"
partition_count = "2"
#failover location MUST be different than location, if same Terraform won't be able to create a Cosmos DB instance
failover_location    = "eastus"
