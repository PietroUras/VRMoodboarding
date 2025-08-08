import requests
import sys
import json
import os
import Api_Token
from datetime import datetime
from PIL import Image
from io import BytesIO

# Extract parameters from command line arguments
subject = sys.argv[1]
aspect_ratio = sys.argv[2]
medium = sys.argv[3]
camera_position = sys.argv[4]
colors = sys.argv[5]
lighting = sys.argv[6]
mood = sys.argv[7]
project_brief = sys.argv[8]
seed = sys.argv[9]

scheduler = 'DPM-Solver'

# Assign scheduler, num_inference_steps, and guidance_scale based on medium
if medium in ['photography', 'film still']:
    scheduler = 'PNDM'
    num_inference_steps, guidance_scale = 15, 6
elif medium in ['painting', 'digital drawing']:
    scheduler = 'DPM-Solver'
    num_inference_steps, guidance_scale = 12, 5
elif medium == 'line art drawing':
    scheduler = 'DPM-Solver'
    num_inference_steps, guidance_scale = 10, 4
elif medium == '3d render':
    scheduler = 'LMS'
    num_inference_steps, guidance_scale = 15, 6
elif medium == 'pixel Art':
    scheduler = 'Euler'
    num_inference_steps, guidance_scale = 10, 4
elif medium == 'motion Graphics':
    scheduler = 'Euler Ancestral'
    num_inference_steps, guidance_scale = 12, 5
else:
    scheduler = 'DPM-Solver'  # Default fallback for scheduler
    num_inference_steps, guidance_scale = 15, 5  # Default fallback for num_inference_steps and guidance_scale

# Set width and height based on aspect ratio
if aspect_ratio == 'Square':
    width, height = 1024, 1024
elif aspect_ratio == 'Portrait':
    width, height = 768, 1024
elif aspect_ratio == 'Landscape':
    width, height = 1024, 768
else:
    raise ValueError(f"Unsupported aspect ratio: {aspect_ratio}")

# Combine all parameters into the subject prompt for a detailed description
if project_brief != "":
    full_prompt = f"{subject},{camera_position}, {colors}, {lighting}, {mood}. this image should be appropriate in a moodboard for the following project {project_brief}"
else :
    full_prompt = f"{subject},{camera_position}, {colors}, {lighting}, {mood}"

negative_prompt = ("blurry, deformed, disfigured, extra fingers, watermark, text, overly detailed, small features, "
                   "fine lines, pixelation, excessive texture")

# Path where to save images
directory_path = "GeneratedImages"

# Replace with your Hugging Face API key
API_TOKEN = Api_Token.API_TOKEN

# Hugging Face API URL for Flux.1dev model
url = "https://api-inference.huggingface.co/models/black-forest-labs/FLUX.1-dev"

headers = {
    "Authorization": f"Bearer {API_TOKEN}",
    "Content-Type": "application/json"
}

data = {
    "inputs": full_prompt,  # Use the full combined prompt
    "parameters": {
        "negative_prompt": negative_prompt,
        "num_inference_steps": num_inference_steps,
        "guidance_scale": guidance_scale,
        "width": width,
        "height": height,
        "seed": seed
    }
}

try:
    # Post request with a timeout of 60 seconds
    response = requests.post(url, headers=headers, data=json.dumps(data), timeout=60)

    # Check if the request was successful
    if response.status_code == 200:
        # Load image from response
        image = Image.open(BytesIO(response.content))

        # Check if output directory exists
        if not os.path.exists(directory_path):
            os.makedirs(directory_path)

        # Save the image without metadata
        filename = f"image_{datetime.now():%m_%d_%H_%M_%S}.png"
        file_path = os.path.join(directory_path, filename)

        image.save(file_path)

        # Output the image path
        print(file_path)  # This will print the saved image path as output

    else:
        print("Error:", response.status_code, response.text)
        sys.exit(1)  # Exit with error code for non-successful response

except requests.exceptions.Timeout:
    print("Error: The request timed out.")
    sys.exit(1)  # Exit with error code for timeout
except Exception as e:
    print("An unexpected error occurred:", str(e))
    sys.exit(1)  # Exit with error code for general errors
