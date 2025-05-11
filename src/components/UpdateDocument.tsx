import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Card, Row, Col } from 'react-bootstrap';
import axios from 'axios';
import '../styles/DocumentForms.css';

interface Document {
  id: number;
  formName: string;
  data: any;
}

interface UpdateDocumentProps {
  documentId?: number;
  onDocumentUpdated: () => void;
}

const UpdateDocument: React.FC<UpdateDocumentProps> = ({ documentId, onDocumentUpdated }) => {
  const [document, setDocument] = useState<Document | null>(null);
  const [formName, setFormName] = useState('');
  const [data, setData] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [fetchLoading, setFetchLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [searchId, setSearchId] = useState<string>('');

  useEffect(() => {
    if (documentId) {
      fetchDocument(documentId);
    }
  }, [documentId]);

  const fetchDocument = async (id: number) => {
    setError('');
    setFetchLoading(true);
    try {
      const response = await axios.get(`https://localhost:7169/api/Document/document-details/${id}`);
      setDocument(response.data);
      setFormName(response.data.formName);
      setData(JSON.stringify(response.data.data, null, 2));
    } catch (error: any) {
      console.error('Error fetching document:', error);
      setError(error.response?.data?.message || `Could not find document with ID: ${id}`);
      setDocument(null);
      setFormName('');
      setData('');
    } finally {
      setFetchLoading(false);
    }
  };

  const handleSearch = () => {
    if (!searchId || isNaN(Number(searchId))) {
      setError('Please enter a valid document ID');
      return;
    }
    fetchDocument(Number(searchId));
  };

  const validateData = (): boolean => {
    try {
      if (data.trim()) {
        JSON.parse(data);
      }
      setError('');
      return true;
    } catch (e) {
      setError('Invalid JSON data format');
      return false;
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (!document) {
      setError('No document loaded for update');
      return;
    }

    if (!formName.trim()) {
      setError('Form name is required');
      return;
    }

    if (!validateData()) {
      return;
    }

    setIsLoading(true);
    
    try {
      const payload = { 
        formName, 
        data: data.trim() ? JSON.parse(data) : {} 
      };
      
      await axios.put(`https://localhost:7169/api/Document/update-document/${document.id}`, payload);
      setSuccess(`Document #${document.id} updated successfully!`);
      
      // Notify parent component that a document was updated
      onDocumentUpdated();
    } catch (error: any) {
      console.error('Error updating document:', error);
      setError(error.response?.data?.message || 'Failed to update document. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container className="document-form-container">
      <Card className="document-form-card">
        <Card.Header as="h2">
          <span className="me-2">✏️</span>
          Update Document
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
                disabled={fetchLoading}
                className="w-100"
              >
                {fetchLoading ? 'Searching...' : '🔍 Search'}
              </Button>
            </Col>
          </Row>

          {success && <div className="alert alert-success">{success}</div>}
          {error && <div className="alert alert-danger">{error}</div>}
          
          {document ? (
            <Form onSubmit={handleSubmit}>
              <div className="mb-3">
                <strong>Document ID:</strong> {document.id}
              </div>
              
              <Form.Group className="mb-3">
                <Form.Label>Form Name</Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Enter form name"
                  value={formName}
                  onChange={(e) => setFormName(e.target.value)}
                  required
                  className="form-input"
                />
              </Form.Group>

              <Form.Group className="mb-3">
                <Form.Label>Data (JSON format)</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={5}
                  placeholder='{"key": "value"}'
                  value={data}
                  onChange={(e) => setData(e.target.value)}
                  className="form-input"
                />
                <Form.Text className="text-muted">
                  Edit document data in valid JSON format
                </Form.Text>
              </Form.Group>

              <div className="d-grid gap-2">
                <Button 
                  variant="success" 
                  type="submit" 
                  disabled={isLoading}
                  className="submit-button"
                >
                  {isLoading ? 'Updating...' : '💾 Update Document'}
                </Button>
                
                <Button 
                  variant="secondary" 
                  onClick={() => {
                    setDocument(null);
                    setFormName('');
                    setData('');
                    setSearchId('');
                    setError('');
                    setSuccess('');
                  }}
                  className="reset-button"
                >
                  🔄 Clear Form
                </Button>
              </div>
            </Form>
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

export default UpdateDocument;