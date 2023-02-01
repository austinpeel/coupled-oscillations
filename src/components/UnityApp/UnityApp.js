import React, { useCallback, useEffect, useState } from 'react';
import { Unity, useUnityContext } from 'react-unity-webgl';
import './UnityApp.css';

function UnityApp(props) {
  const {
    unityProvider,
    isLoaded,
    loadingProgression,
    requestFullscreen,
    addEventListener,
    removeEventListener,
    sendMessage,
  } = useUnityContext({
    loaderUrl: props.loaderUrl,
    dataUrl: props.dataUrl,
    frameworkUrl: props.frameworkUrl,
    codeUrl: props.codeUrl,
  });

  const loadingPercentage = Math.round(loadingProgression * 100);

  const [unityIsFullscreen, setUnityIsFullscreen] = useState(false);

  const handleEnterFullScreen = useCallback(() => {
    requestFullscreen(true);
    setUnityIsFullscreen(true);
  }, [requestFullscreen, setUnityIsFullscreen]);

  useEffect(() => {
    addEventListener('EnterFullscreen', handleEnterFullScreen);
    return () => {
      removeEventListener('EnterFullscreen', handleEnterFullScreen);
    };
  }, [addEventListener, removeEventListener, handleEnterFullScreen]);

  const handleExitFullScreen = useCallback(() => {
    requestFullscreen(false);
    setUnityIsFullscreen(false);
  }, [requestFullscreen, setUnityIsFullscreen]);

  useEffect(() => {
    addEventListener('ExitFullscreen', handleExitFullScreen);
    return () => {
      removeEventListener('ExitFullscreen', handleExitFullScreen);
    };
  }, [addEventListener, removeEventListener, handleExitFullScreen]);

  // Handle when the Esc button is pressed from fullscreen mode
  const [isFullscreen, setIsFullscreen] = useState(false);

  useEffect(() => {
    function onFullscreenChange() {
      setIsFullscreen(Boolean(document.fullscreenElement));
      if (isFullscreen === unityIsFullscreen) {
        // console.log('Sending message to Unity');
        sendMessage('Footer', 'HandleExitFullscreenFromBrowser');
      }
    }
    document.addEventListener('fullscreenchange', onFullscreenChange);
    return () =>
      document.removeEventListener('fullscreenchange', onFullscreenChange);
  }, [setIsFullscreen, isFullscreen, unityIsFullscreen, sendMessage]);

  return (
    <div id='unity-app' className='unity-app'>
      {!isLoaded && (
        <div className='loading-overlay'>
          <p>Loading... ({loadingPercentage}%)</p>
        </div>
      )}
      <Unity unityProvider={unityProvider} className='unity' />
      {/* <button onClick={handleClickEnterFullscreen}>Full screen</button> */}
    </div>
  );
}

export default UnityApp;
