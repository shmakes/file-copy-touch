# file-copy-touch
## Easy touch screen app to copy a file between two static locations

This application will provide a touch-screen-friendly UI for the user.

There will be a text box the the user can either type in from the keyboard or us a barcode scanner to enter a short string of characters.

When the user has completed typing the configured number of characters (minInputLength) for a configurable dwell time (debounceMilliseconds) - the program will use the letters to filter the files listed in the configured source directory (sourceDirectory).

The matching files will be displayed in a list of button type entities that are easy to read and to interact with on a touch screen.

If the user does not see the file that they were looking for, they can press a button near the entry text box to clear the string of characters and the results.

If the user touches a file in the results, the program will clear any existing files in the target directory (targetDirectory) and then copy the selected file from the source to the target directory.

When the copy function is complete the user will receive a confirmation prompt and when they touch the confirmation button, the input text box and the results will clear, returning the application to its initial state.

Configuration will be read from a file in the same directory the the application is running.

The configurable options will be:

- minimumInputLength: an integer number between 5 and 20 defaulting to 5.
- debounceMilliseconds: an integer number between 100 and 3000 defaulting to 500.
- sourceDirectory: a string representing a path to the source files
- targetDirectory: a string representing a path to the target file location.

The application should run in the current logged in Windows user's context and should work with local, mapped or UNC drive specifications.
