import React, { useState, useEffect } from 'react';
import { Container } from 'react-bootstrap';
import axios from 'axios';
import './App.css';
import Dashboard from './components/Dashboard';
import DocumentList from './components/DocumentList';
import DocumentSearch from './components/DocumentSearch';
import AddDocument from './components/AddDocument';
import UpdateDocument from './components/UpdateDocument';
import DocumentDetails from './components/DocumentDetails';

const App: React.FC = () => {
  const [documents, setDocuments] = useState<any[]>([]);
  const [selectedDocument, setSelectedDocument] = useState<any>(null);
  const [activeComponent, setActiveComponent] = useState<string>('dashboard');

  const fetchDocuments = async () => {
    try {
      const response = await axios.get('https://localhost:7169/api/Document/get-all-documents');
      setDocuments(response.data);
    } catch (error) {
      console.error('Error fetching documents:', error);
    }
  };

  useEffect(() => {
    fetchDocuments();
  }, []);

  const deleteDocument = async (id: number) => {
    try {
      await axios.delete(`https://localhost:7169/api/Document/delete-document/${id}`);
      fetchDocuments();
      alert('Document deleted successfully!');
    } catch (error) {
      console.error('Error deleting document:', error);
      alert('Failed to delete document.');
    }
  };

  const getDocumentDetails = async (id: number) => {
    setSelectedDocument(id);
    setActiveComponent('details');
  };

  const renderActiveComponent = () => {
    switch (activeComponent) {
      case 'dashboard':
        return (
          <Dashboard 
            onAddDocument={() => setActiveComponent('add')}
            onUpdateDocument={() => setActiveComponent('update')}
            onDeleteDocument={() => setActiveComponent('list')}
            onDetailsDocument={() => setActiveComponent('details')}
            onGetAllDocuments={() => setActiveComponent('list')}
            onGetByName={() => setActiveComponent('search')}
          />
        );
      case 'list':
        return (
          <DocumentList 
            documents={documents}
            setDocuments={setDocuments}
            onDetails={getDocumentDetails}
            onDelete={deleteDocument}
          />
        );
      case 'add':
        return <AddDocument onDocumentAdded={fetchDocuments} />;
      case 'update':
        return <UpdateDocument onDocumentUpdated={fetchDocuments} documentId={selectedDocument} />;
      case 'details':
        return <DocumentDetails documentId={selectedDocument} />;
      case 'search':
        return <DocumentSearch setDocuments={setDocuments} />;
      default:
        return <Dashboard 
          onAddDocument={() => setActiveComponent('add')}
          onUpdateDocument={() => setActiveComponent('update')}
          onDeleteDocument={() => setActiveComponent('list')}
          onDetailsDocument={() => setActiveComponent('details')}
          onGetAllDocuments={() => setActiveComponent('list')}
          onGetByName={() => setActiveComponent('search')}
        />;
    }
  };

  return (
    <Container fluid className="p-0">
      <header className="app-header text-white py-3 px-4 d-flex justify-content-between align-items-center">
        <h1 className="m-0 app-title" onClick={() => setActiveComponent('dashboard')} style={{ cursor: 'pointer' }}>AutoDocAi</h1>
        <div>
          <button 
            className="btn btn-outline-light me-2" 
            onClick={() => setActiveComponent('dashboard')}
          >
            Home
          </button>
        </div>
      </header>
      
      <main>
        {renderActiveComponent()}
      </main>
      
      <footer className="bg-dark text-white text-center py-3 mt-5">
        <p className="m-0">© {new Date().getFullYear()} AutoDocAi - Document Management System</p>
      </footer>
    </Container>
  );
};

export default App;
