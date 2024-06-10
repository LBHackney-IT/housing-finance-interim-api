# Setup

## AWS Hosted Databases:
Can be launched from the root of the repository via its Makefile commands. To do so:
- Edit the `PROFILE` variable in the Makefile to match your AWS CLI profile name
- Refresh your AWS CLI session with `aws sso login` or `make sso_login` if needed
- Port forward to the EE database using the Makefile command `make port_forward_ee` (see comments in Makefile for details). **You must use Git Bash or WSL on Windows to run this command**
- While port forwarding in a separate process, run `make test-db` to run the tests, or use a test runner extension in VSCode or other IDE

NOTE: The CLI profile in the Makefile must be the same one Pytest uses to connect to the database. Check `conftest.py` for the default profile name, or pass the command line argument `pytest --aws-profile <profile>` as in the Makefile to override it.
