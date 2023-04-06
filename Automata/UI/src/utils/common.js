export function timeout(ms) {
  return new Promise((res) => setTimeout(res, ms));
}
export async function saveCanvas(canvas, filename = 'canvas.jpg') {
  let blob = await new Promise((resolve) =>
    canvas.toBlob(resolve, 'image/jpeg')
  );
  var blobUrl = URL.createObjectURL(blob);
  var link = document.createElement('a');
  link.href = blobUrl;
  link.download = filename;
  link.innerHTML = '';
  document.body.appendChild(link);
  link.click();
  link.remove();
}

export async function getCanvasAsFile(canvas, filename = 'canvas.jpg') {
  let blob = await new Promise((resolve) =>
    canvas.toBlob(resolve, 'image/jpeg')
  );
  return new File([blob], filename, {
    type: 'image/png',
    lastModified: new Date().getTime(),
  });
}
