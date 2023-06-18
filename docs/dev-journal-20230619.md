# Development Journal

## 2023-06-19

### Progress

- Added "max parallelization" (with a default value of `5`) to the import config options. The import tool previously maximized machine resource usage; this allows the user to have more control. The tradeoff is longer run times.
- Refactored symbol meta data to preserve the meta data at the end of the cycle and to also fetch from a new table built for this specific purpose.

