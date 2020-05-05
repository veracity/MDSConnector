import requests

# cert_file_path = r"../MDSClient/clientCertificates/root_ca_dnvgl_dev.pfx"
cert_path = "cert.pem"
key_path = "decryptedKey.pem"
url = r"https://localhost:10001"

res = requests.get(url, verify=False, cert=[cert_path, key_path])

print(res)