import React, { useState } from 'react';
import { Container, Form, Button, Card } from 'react-bootstrap';
import axios from 'axios';
import '../styles/DocumentForms.css';

interface AddDocumentProps {
  onDocumentAdded: () => void;
}

const AddDocument: React.FC<AddDocumentProps> = ({ onDocumentAdded }) => {
  const [formName, setFormName] = useState('');
  const [data, setData] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

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
      
      await axios.post('https://localhost:7169/api/Document/seed-document', payload);
      setSuccess('Document added successfully!');
      setFormName('');
      setData('');
      
      // Notify parent component that a document was added
      onDocumentAdded();
    } catch (error: any) {
      console.error('Error adding document:', error);
      setError(error.response?.data?.message || 'Failed to add document. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container className="document-form-container">
      <Card className="document-form-card">
        <Card.Header as="h2">
          <span className="me-2">➕</span>
          Add New Document
        </Card.Header>
        <Card.Body>
          {success && <div className="alert alert-success">{success}</div>}
          {error && <div className="alert alert-danger">{error}</div>}
          
          <Form onSubmit={handleSubmit}>
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
                Enter document data in valid JSON format
              </Form.Text>
            </Form.Group>

            <div className="d-grid gap-2">
              <Button 
                variant="primary" 
                type="submit" 
                disabled={isLoading}
                className="submit-button"
              >
                {isLoading ? 'Adding...' : '💾 Add Document'}
              </Button>
              
              <Button 
                variant="secondary" 
                onClick={() => {
                  setFormName('');
                  setData('');
                  setError('');
                  setSuccess('');
                }}
                className="reset-button"
              >
                🔄 Reset Form
              </Button>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default AddDocument;