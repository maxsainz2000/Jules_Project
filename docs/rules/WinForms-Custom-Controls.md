# WinForms Custom Controls Guidelines

## DataGridView Date Pickers
WinForms does not have a native `DataGridViewDateTimePickerColumn`. When a specification calls for a "date picker" or "DatePicker" inside a grid (translated from WPF's `CellEditingTemplate`), you MUST implement a custom `DataGridViewColumn` and `DataGridViewCell` that hosts a `DateTimePicker` control when editing.

Do not substitute a standard `DataGridViewTextBoxColumn` for date fields when a picker is explicitly requested.

### Implementation Requirements:
1. Create a `DataGridViewDateTimePickerColumn` class inheriting from `DataGridViewColumn`.
2. Create a `DataGridViewDateTimePickerCell` class inheriting from `DataGridViewTextBoxCell`.
3. Create a `DataGridViewDateTimePickerEditingControl` class inheriting from `DateTimePicker` and implementing `IDataGridViewEditingControl`.
4. Register the cell type in the column constructor.
5. Use this custom column type in the view designer code for the relevant date columns.
