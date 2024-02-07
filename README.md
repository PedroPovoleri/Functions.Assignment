# Solution Overview

## Function One - City Weather Validation and Forecast Retrieval

The first Azure function is designed to handle incoming messages containing a city name. Upon receiving a message, the function performs the following tasks:

1. **Message Validation:**
   - Reads the incoming message and validates the data.
   - Checks if a city name is present.
   - Validates that the city name is alphanumeric.
   - Ensures that the city name does not exceed 100 characters.

2. **Weather API Integration:**
   - If the data is valid, calls the weather forecast API using the city from the message.
   - Processes the API response.

3. **Postman Echo API:**
   - If a valid response is received from the Weather API, the results are sent to the Postman echo API.

4. **Error Handling:**
   - If any validation fails or other errors occur, an error response is sent to the response queue.

5. **Success Handling:**
   - If the entire process completes without errors, a success message is sent to the response queue.

## Function Two - Logging Responses

The second Azure function is responsible for handling messages containing success or failure responses from Function One. The tasks performed by this function include:

1. **Response Logging:**
   - Receives messages with success or failure responses from Function One.
   - Logs the responses using a logger.

This two-function solution ensures efficient handling of city weather data, validation, and logging of responses for further analysis and monitoring.
