terraform {
  required_version = "=0.14.7"
  #choco upgrade terraform -y

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.49.0"
    }
  }
}

provider "azurerm" {
  features {}
}
locals {
  demoName             = "logging"
  env                  = "dev"
  rgName               = "f2calv${local.demoName}${local.env}"
  location             = "West Europe"
  appinsights_name     = "f2calv${local.demoName}${local.env}"
  storage_account_name = "f2calv${local.demoName}${local.env}"
  tags                 = { workspace = terraform.workspace, environment = local.env }
}

resource "azurerm_resource_group" "rg" {
  #   lifecycle {
  #     prevent_destroy = true
  #   }
  name     = local.rgName
  location = local.location
}


#https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/application_insights
resource "azurerm_application_insights" "this" {
  name                 = local.appinsights_name
  location             = azurerm_resource_group.rg.location
  resource_group_name  = azurerm_resource_group.rg.name
  application_type     = "web"
  daily_data_cap_in_gb = 1  #default 100
  retention_in_days    = 30 #default 90
  tags                 = local.tags
}

#https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/storage_account
resource "azurerm_storage_account" "this" {
  name                      = local.storage_account_name
  location                  = azurerm_resource_group.rg.location
  resource_group_name       = azurerm_resource_group.rg.name
  account_kind              = "StorageV2" #default is StorageV2
  account_tier              = "Standard"
  account_replication_type  = "LRS"
  access_tier               = "Hot" #default is Hot
  enable_https_traffic_only = true
  allow_blob_public_access  = false
  min_tls_version           = "TLS1_2"
  tags                      = local.tags
  identity {
    type = "SystemAssigned"
  }
}
