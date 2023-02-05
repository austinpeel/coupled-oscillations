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

  const handleEnterFullscreen = useCallback(() => {
    requestFullscreen(true);
    setUnityIsFullscreen(true);
  }, [requestFullscreen, setUnityIsFullscreen]);

  useEffect(() => {
    addEventListener('EnterFullscreen', handleEnterFullscreen);
    return () => {
      removeEventListener('EnterFullscreen', handleEnterFullscreen);
    };
  }, [addEventListener, removeEventListener, handleEnterFullscreen]);

  const handleExitFullscreen = useCallback(() => {
    requestFullscreen(false);
    setUnityIsFullscreen(false);
  }, [requestFullscreen, setUnityIsFullscreen]);

  useEffect(() => {
    addEventListener('ExitFullscreen', handleExitFullscreen);
    return () => {
      removeEventListener('ExitFullscreen', handleExitFullscreen);
    };
  }, [addEventListener, removeEventListener, handleExitFullscreen]);

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
      <button onClick={handleEnterFullscreen}>Full screen</button>
    </div>
  );
}

export default UnityApp;
