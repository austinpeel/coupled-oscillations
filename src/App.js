import './App.css';
import Header from './components/Header/Header';
// import Footer from './components/Footer/Footer';
import UnityApp from './components/UnityApp/UnityApp';

const appData = {
  loaderUrl: 'UnityGame/Build/UnityGame.loader.js',
  dataUrl: 'UnityGame/Build/UnityGame.data',
  frameworkUrl: 'UnityGame/Build/UnityGame.framework.js',
  codeUrl: 'UnityGame/Build/UnityGame.wasm',
  description: 'Coupled oscillations...',
};

function App() {
  return (
    <div className='container'>
      <Header />
      <UnityApp {...appData} />
      {/* <Footer /> */}
    </div>
  );
}

export default App;
