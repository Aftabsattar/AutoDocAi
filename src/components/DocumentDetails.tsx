import React, { useState, useEffect } from 'react';
import { Container, Card, Button, Form, Row, Col, Badge } from 'react-bootstrap';
import axios from 'axios';
import '../styles/DocumentForms.css';

interface Document {
  id: number;
  formName: string;
  data: any;
}

interface DocumentDetailsProps {
  documentId?: number;
}

const DocumentDetails: React.FC<DocumentDetailsProps> = ({ documentId }) => {
  const [document, setDocument] = useState<Document | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [searchId, setSearchId] = useState<string>('');

  useEffect(() => {
    if (documentId) {
      fetchDocument(documentId);
    }
  }, [documentId]);

  const fetchDocument = async (id: number) => {
    setError('');
    setIsLoading(true);
    try {
      const response = await axios.get(`https://localhost:7169/api/Document/document-details/${id}`);
      setDocument(response.data);
    } catch (error: any) {
      console.error('Error fetching document:', error);
      setError(error.response?.data?.message || `Could not find document with ID: ${id}`);
      setDocument(null);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSearch = () => {
    if (!searchId || isNaN(Number(searchId))) {
      setError('Please enter a valid document ID');
      return;
    }
    fetchDocument(Number(searchId));
  };

  const renderJsonData = (data: any) => {
    try {
      return (
        <pre className="json-display">
          {JSON.stringify(data, null, 2)}
        </pre>
      );
    } catch (e) {
      return <p className="text-danger">Error parsing data</p>;
    }
  };

  return (
    <Container className="document-form-container">
      <Card className="document-form-card">
        <Card.Header as="h2">
          <span className="me-2">🔍</span>
          Document Details
        </Card.Header>
        <Card.Body>
          {/* Document ID Search */}
          <Row className="mb-4">
            <Col md={8}>
              <Form.Group>
                <Form.Label>Search Document by ID</Form.Label>
                <Form.Control 
                  type="number" 
                  placeholder="Enter document ID"
                  value={searchId}
                  onChange={(e) => setSearchId(e.target.value)}
                  className="form-input"
                />
              </Form.Group>
            </Col>
            <Col md={4} className="d-flex align-items-end">
              <Button 
                variant="info" 
                onClick={handleSearch} 
                disabled={isLoading}
                className="w-100"
              >
                {isLoading ? 'Searching...' : '🔎 Search'}
              </Button>
            </Col>
          </Row>

          {error && <div className="alert alert-danger">{error}</div>}
          
          {isLoading ? (
            <div className="text-center p-5">
              <p>Loading document details...</p>
            </div>
          ) : document ? (
            <>
              <div className="document-details">
                <Row className="mb-3">
                  <Col md={3}><strong>Document ID:</strong></Col>
                  <Col md={9}>
                    <Badge bg="primary" className="p-2">{document.id}</Badge>
                  </Col>
                </Row>
                
                <Row className="mb-3">
                  <Col md={3}><strong>Form Name:</strong></Col>
                  <Col md={9}>{document.formName}</Col>
                </Row>
                
                <Row>
                  <Col md={12}>
                    <strong>Document Data:</strong>
                    <div className="mt-2 p-3 bg-light rounded">
                      {renderJsonData(document.data)}
                    </div>
                  </Col>
                </Row>
              </div>
              
              <div className="mt-4 d-grid">
                <Button 
                  variant="secondary" 
                  onClick={() => {
                    setDocument(null);
                    setSearchId('');
                    setError('');
                  }}
                >
                  🔄 Clear
                </Button>
              </div>
            </>
          ) : (
            <div className="text-center p-4">
              <p>No document loaded. Please search for a document by ID.</p>
            </div>
          )}
        </Card.Body>
      </Card>
    </Container>
  );
};

export default DocumentDetails;