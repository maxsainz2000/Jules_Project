# Jules AI Coding Guidelines

## CRITICAL: No Empty Commits
- When creating new files, you MUST implement the actual logic and UI components before committing and pushing the PR. 
- Creating empty files as placeholders (e.g., from a plan.md) and submitting them is strictly forbidden. 
- Always verify that the file contents are saved and contain the required implementations before finishing the task.

## CRITICAL: Git Staging New Files
- When you create NEW files, you MUST explicitly stage them using `git add <file>` or `git add .` before you run `git commit`. 
- If you only run `git commit -am` or `git commit -a`, it will NOT track newly created files, resulting in PRs that omit all of your work.
- Always run `git status` right before committing to ensure all your new files are listed under "Changes to be committed".
