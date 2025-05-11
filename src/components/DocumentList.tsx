import React, { useState, useEffect } from 'react';
import { Container, Table, Button, Card, Form, InputGroup } from 'react-bootstrap';
import axios from 'axios';
import '../styles/DocumentForms.css';

interface Document {
  id: number;
  formName: string;
  data: any;
}

interface DocumentListProps {
  onDetails: (id: number) => void;
  onDelete: (id: number) => void;
  documents: Document[];
  setDocuments: React.Dispatch<React.SetStateAction<Document[]>>;
}

const DocumentList: React.FC<DocumentListProps> = ({ 
  onDetails, 
  onDelete, 
  documents, 
  setDocuments 
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [filter, setFilter] = useState('');
  
  const fetchDocuments = async () => {
    setIsLoading(true);
    try {
      const response = await axios.get('https://localhost:7169/api/Document/get-all-documents');
      setDocuments(response.data);
    } catch (error) {
      console.error('Error fetching documents:', error);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchDocuments();
  }, []);

  const filteredDocuments = documents.filter(doc => 
    doc.formName.toLowerCase().includes(filter.toLowerCase()) || 
    doc.id.toString().includes(filter)
  );

  return (
    <Container className="document-form-container">
      <Card className="document-form-card">
        <Card.Header as="h2" className="d-flex justify-content-between align-items-center">
          <span>Document List</span>
          <Button 
            variant="primary" 
            onClick={fetchDocuments} 
            disabled={isLoading}
            className="btn-sm"
          >
            {isLoading ? 'Refreshing...' : '🔄 Refresh'}
          </Button>
        </Card.Header>
        <Card.Body>
          <InputGroup className="mb-3">
            <Form.Control
              placeholder="Filter by ID or form name"
              value={filter}
              onChange={(e) => setFilter(e.target.value)}
            />
            {filter && (
              <Button 
                variant="outline-secondary" 
                onClick={() => setFilter('')}
              >
                Clear
              </Button>
            )}
          </InputGroup>

          {documents.length === 0 ? (
            <div className="text-center p-5">
              <p>No documents found. Click refresh to load documents.</p>
            </div>
          ) : filteredDocuments.length === 0 ? (
            <div className="text-center p-5">
              <p>No documents match your filter criteria.</p>
            </div>
          ) : (
            <div className="table-responsive">
              <Table hover className="document-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Form Name</th>
                    <th className="text-center">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredDocuments.map((doc) => (
                    <tr key={doc.id}>
                      <td>{doc.id}</td>
                      <td>{doc.formName}</td>
                      <td className="text-center">
                        <Button
                          variant="info"
                          onClick={() => onDetails(doc.id)}
                          className="me-2 btn-sm"
                          title="View Details"
                        >
                          <span className="me-1">🔍</span> Details
                        </Button>
                        <Button
                          variant="danger"
                          onClick={() => {
                            if (window.confirm('Are you sure you want to delete this document?')) {
                              onDelete(doc.id);
                            }
                          }}
                          className="btn-sm"
                          title="Delete Document"
                        >
                          <span className="me-1">🗑️</span> Delete
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>
          )}
          
          <div className="mt-3 text-muted small">
            {filteredDocuments.length} document(s) displayed • {documents.length} total
          </div>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default DocumentList;