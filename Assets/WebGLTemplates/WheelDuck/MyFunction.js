function FullScreen() {
  console.log("FullScreen");
  var value = window.uinstance;
  console.log("Global variable value:", value);
  value.SetFullscreen(1);
}

function GetJSON(key, objectName, callback, fallback) {
  try {
    window.GetFunc(key, objectName, callback);
  } catch (error) {
    window.uinstance.SendMessage(
      objectName,
      fallback,
      JSON.stringify(error, Object.getOwnPropertyNames(error))
    );
  }
}

function SetJSON(key, value, objectName, callback, fallback) {
  try {
    window.SetData(key, value, objectName, callback);
  } catch (error) {
    window.uinstance.SendMessage(
      objectName,
      fallback,
      JSON.stringify(error, Object.getOwnPropertyNames(error))
    );
  }
}
