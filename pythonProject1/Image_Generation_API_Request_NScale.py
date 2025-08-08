import sys
import os
import base64
from datetime import datetime
import openai
import Api_Token

# Extract parameters from command line arguments
subject = sys.argv[1]
aspect_ratio = sys.argv[2]
medium = sys.argv[3]  # currently unused but kept for compatibility
camera_position = sys.argv[4]
colors = sys.argv[5]
lighting = sys.argv[6]
mood = sys.argv[7]
project_brief = sys.argv[8]
seed = sys.argv[9]

# Set width and height based on aspect ratio
if aspect_ratio == 'Square':
    width, height = 1024, 1024
elif aspect_ratio == 'Portrait':
    width, height = 768, 1024
elif aspect_ratio == 'Landscape':
    width, height = 1024, 768
else:
    raise ValueError(f"Unsupported aspect ratio: {aspect_ratio}")

# Build full prompt
if project_brief != "":
    full_prompt = f"{subject}, {camera_position}, {colors}, {lighting}, {mood}. This image should be appropriate in a moodboard for the following project: {project_brief}."
else:
    full_prompt = f"{subject}, {camera_position}, {colors}, {lighting}, {mood}"

# Image saving directory
directory_path = "GeneratedImages"
os.makedirs(directory_path, exist_ok=True)

# Set up OpenAI client for NScale
nscale_api_key = Api_Token.NSCALE_API_KEY
nscale_base_url = "https://inference.api.nscale.com/v1"

client = openai.OpenAI(
    api_key=nscale_api_key,
    base_url=nscale_base_url
)

try:
    # Generate image
    response = client.images.generate(
        model="black-forest-labs/FLUX.1-schnell",
        prompt=full_prompt,
        size=f"{width}x{height}",
        n=1,
    )

    # Decode base64 image
    image_base64 = response.data[0].b64_json
    image_data = base64.b64decode(image_base64)

    # Save image
    filename = f"image_{datetime.now():%m_%d_%H_%M_%S}.png"
    file_path = os.path.join(directory_path, filename)
    with open(file_path, "wb") as f:
        f.write(image_data)

    print(file_path)

except Exception as e:
    print(f"Error during image generation: {e}")
    sys.exit(1)
