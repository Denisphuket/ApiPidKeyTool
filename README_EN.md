# ApiPidKeyTool
[Версия на русском](README.md)

## PidKey Key Check API

## Description

This project is an API created for checking product keys using the external tool `PidKey.exe`. It allows users to submit product keys and receive information about their status and other details.

## How It Works

The API runs `PidKey.exe` with the provided product keys and returns a formatted response with information about each key. Responses include data such as description, remaining activations, error code, and more.

## Usage Examples

To use the API, users can send GET requests to the following endpoints:

- `/check-keys?keys=<key1>,<key2>,...`: This endpoint accepts one or more keys separated by commas and returns information about each key.
- `/get-cid?installationId=<installationId>`: This endpoint accepts an installation ID and returns the corresponding CID.

## Endpoints

- `GET /check-keys`: Accepts a string of keys and returns information about each key.
- `GET /get-cid`: Accepts an installation ID and returns CID.

## Acknowledgements

Special thanks to [@laomms](https://github.com/laomms) for creating such a useful tool, `PidKey.exe`.

## License

MIT
