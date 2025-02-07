# FINOS CCC Validation

Project to validate FINOS CCC common features, threats and controls usage in service files

## Common Features, Threats and Controls

The validator checks to see if there are any duplicate ids.

## Features

The validator checks every `features.yaml` file for the following:

- Common features listed in file are defined in the `common-features.yaml` file.
- Validates each feature id starts with the id defined in the corresponding `metadata.yaml` file.

## Threats

The validator checks every `threats.yaml` file for the following:

- Common Threats listed in file are defined in the `common-threats.yaml` file.
- Validates each threat id starts with the id defined in the corresponding `metadata.yaml` file.
- Validates that each `feature` listed against each `threat` is defined in either the `common-features.yaml` or the corresponding `features.yaml` file.

## Controls

The validator checks every `controls.yaml` file for the following:

- Common Controls listed in file are defined in the `common-controls.yaml` file.
- Validates each control id starts with the id defined in the corresponding `metadata.yaml` file.
- Validates that each `threat` listed against each `control` is defined in either the `common-threats.yaml` or the corresponding `threats.yaml` file.
- Validates each test requirement id starts with the corresponding control id.