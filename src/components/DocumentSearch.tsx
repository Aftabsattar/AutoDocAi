import React, { useState } from 'react';
import { Container, Form, Button, Card, Row, Col, Alert } from 'react-bootstrap';
import axios from 'axios';
import '../styles/DocumentForms.css';

interface DocumentSearchProps {
  setDocuments: React.Dispatch<React.SetStateAction<any[]>>;
}

const DocumentSearch: React.FC<DocumentSearchProps> = ({ setDocuments }) => {
  const [searchFormName, setSearchFormName] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const fetchDocuments = async () => {
    setIsLoading(true);
    setError('');
    setSuccess('');
    
    try {
      const response = await axios.get('https://localhost:7169/api/Document/get-all-documents');
      setDocuments(response.data);
      setSuccess(`All documents loaded (${response.data.length} total)`);
    } catch (error) {
      console.error('Error fetching documents:', error);
      setError('Failed to load all documents. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const getDocumentByName = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    
    if (!searchFormName.trim()) {
      setError('Please enter a form name to search');
      return;
    }
    
    setIsLoading(true);
    
    try {
      const response = await axios.get('https://localhost:7169/api/Document/by-name', {
        params: { formName: searchFormName }
      });
      
      if (response.data && response.data.length > 0) {
        setDocuments(response.data);
        setSuccess(`Found ${response.data.length} document(s) matching "${searchFormName}"`);
      } else {
        setDocuments([]);
        setError(`No documents found with name "${searchFormName}"`);
      }
    } catch (error: any) {
      console.error('Error fetching document by name:', error);
      setError(error.response?.data?.message || 'Failed to fetch documents. Please try again.');
      setDocuments([]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container className="document-form-container">
      <Card className="document-form-card">
        <Card.Header as="h2">Search Documents</Card.Header>
        <Card.Body>
          {success && <Alert variant="success">{success}</Alert>}
          {error && <Alert variant="danger">{error}</Alert>}
          
          <Form onSubmit={getDocumentByName}>
            <Row className="align-items-end">
              <Col md={8}>
                <Form.Group className="mb-3">
                  <Form.Label>Form Name</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Enter form name to search"
                    value={searchFormName}
                    onChange={(e) => setSearchFormName(e.target.value)}
                    className="form-input"
                  />
                </Form.Group>
              </Col>
              <Col md={4}>
                <div className="d-grid gap-2 mb-3">
                  <Button 
                    variant="primary" 
                    type="submit"
                    disabled={isLoading}
                    className="submit-button"
                  >
                    {isLoading ? 'Searching...' : 'Search'}
                  </Button>
                </div>
              </Col>
            </Row>
          </Form>
          
          <hr className="my-4" />
          
          <div className="d-flex flex-column flex-sm-row justify-content-between">
            <p className="mb-3 mb-sm-0">
              <small className="text-muted">
                Not sure what to search for? Load all documents and filter them:
              </small>
            </p>
            <Button 
              variant="outline-secondary" 
              onClick={fetchDocuments}
              disabled={isLoading}
            >
              Show All Documents
            </Button>
          </div>
          
        </Card.Body>
      </Card>
    </Container>
  );
};

export default DocumentSearch;