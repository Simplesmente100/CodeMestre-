# Resources Folder

This folder contains icons and other resources for the Revit AI Assistant plugin.

## Icons

To add custom icons for the plugin buttons:

1. Create 32x32 pixel PNG images for ribbon buttons
2. Name them appropriately (e.g., `ai_icon_32.png`, `help_icon_32.png`)
3. Set Build Action to "Resource" in Visual Studio
4. Update the Application.cs file to reference the icons

## Example Icon Names

- `ai_icon_32.png` - Main AI Assistant button (32x32)
- `ai_icon_16.png` - Small AI Assistant button (16x16)
- `help_icon_32.png` - Help button (32x32)
- `settings_icon_32.png` - Settings button (32x32)

## Creating Icons

You can create simple icons using:
- Online icon generators
- GIMP, Photoshop, or similar tools
- Icon fonts (Font Awesome, Material Icons)

The plugin will work without custom icons, using default Revit button styles.