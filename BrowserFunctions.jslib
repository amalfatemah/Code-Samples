mergeInto(LibraryManager.library, {

DetectExtension: function (extensionName) {
    if (UTF8ToString(extensionName) === "MetaMask" && typeof window.ethereum !== "undefined" && window.ethereum.isMetaMask) {
      return true;
    } else {
      return false;
    }
  },



DetectSecondInstance: function () {
  var token = 'CCG_SECOND_INSTANCE';

  function handleBeforeUnload() {
    console.log("Before Unload");
      if (typeof localStorage !== 'undefined') {
         var num_Inst_Unload = parseInt(localStorage.getItem(token));
         if(num_Inst_Unload == 1){
            localStorage.removeItem(token);
         }
         else{
            localStorage.setItem(token, (numInstances - 1));
         }
      }
  }

  window.addEventListener('beforeunload', handleBeforeUnload);

  if (typeof localStorage !== 'undefined') {
    if (localStorage.getItem(token)) {
      window.alert("The game is already running on another page!");
      var numInstances = (parseInt(localStorage.getItem(token)) + 1);
      console.log(numInstances)
      localStorage.setItem(token, numInstances);

      return true;  // Second instance detected
    } else {
      localStorage.setItem(token, 1);
    }
  }
  return false; // Second instance not detected
},

CloseWindow: function(){
    console.log("Redirect the current window");
    window.location.href = "https://cryptochampion.game/";
}

});