# Development Journal

## 2022-11-17

### Progress

1. Got the yaml configuration working; still might be some tweaks required, but the mechanics of YAML serialization are sorted.
    1. Special thanks to [YAMLDotNet](https://github.com/aaubry/YamlDotNet).
    1. I tried to use `dynamic` and `ExpandoObject` to parse the files, but that was a bust.
    1. Building an `ImportConfiguration` that can be transformed from the YAML file was much smoother and more predictable.
    1. The `Import.AppServices.Tests.ImportConfigurationTests.Serialization_Deserialization` test is worthing reviewing.

### Next Steps

1. Transform the YAML config into "action items" that are unique and properly sorted.
1. Retrieve undone "action items" from the data store and insert them into the collection.
1. The final product should have no dupes.