terraform {
  backend "azurerm" {
    # modify the values for the storage account
    resource_group_name  = "rg-sagalogic-tf"
    storage_account_name = "sttfsagaorch"
    container_name       = "testingterraform"
    key                  = "terraform.tfstate"
    use_oidc             = true
  }
}