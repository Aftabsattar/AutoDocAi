import React from 'react';
import { Container, Row, Col, Button, Card } from 'react-bootstrap';
import '../styles/Dashboard.css';

interface DashboardProps {
  onAddDocument: () => void;
  onDeleteDocument: () => void;
  onUpdateDocument: () => void;
  onDetailsDocument: () => void;
  onGetAllDocuments: () => void;
  onGetByName: () => void;
}

const Dashboard: React.FC<DashboardProps> = ({
  onAddDocument,
  onDeleteDocument,
  onUpdateDocument,
  onDetailsDocument,
  onGetAllDocuments,
  onGetByName
}) => {
  return (
    <div className="dashboard-container">
      <div className="dashboard-overlay">
        <Container>
          <Row className="mb-4">
            <Col>
              <h1 className="text-center dashboard-title">AutoDocAi Dashboard</h1>
              <p className="text-center dashboard-subtitle">
                Manage your documents efficiently with our intelligent document automation system
              </p>
            </Col>
          </Row>
          
          <Row className="dashboard-cards">
            <Col md={4} className="mb-4">
              <Card className="dashboard-card">
                <Card.Body>
                  <Card.Title>Document Creation</Card.Title>
                  <Card.Text>Add new documents to the system</Card.Text>
                  <Button variant="primary" onClick={onAddDocument} className="dashboard-button">
                    <span className="me-2">➕</span> Add Document
                  </Button>
                </Card.Body>
              </Card>
            </Col>
            
            <Col md={4} className="mb-4">
              <Card className="dashboard-card">
                <Card.Body>
                  <Card.Title>Document Modification</Card.Title>
                  <Card.Text>Update existing documents in the system</Card.Text>
                  <Button variant="success" onClick={onUpdateDocument} className="dashboard-button">
                    <span className="me-2">✏️</span> Update Document
                  </Button>
                </Card.Body>
              </Card>
            </Col>
            
            <Col md={4} className="mb-4">
              <Card className="dashboard-card">
                <Card.Body>
                  <Card.Title>Document Removal</Card.Title>
                  <Card.Text>Delete documents from the system</Card.Text>
                  <Button variant="danger" onClick={onDeleteDocument} className="dashboard-button">
                    <span className="me-2">🗑️</span> Delete Document
                  </Button>
                </Card.Body>
              </Card>
            </Col>
          </Row>
          
          <Row className="dashboard-cards">
            <Col md={4} className="mb-4">
              <Card className="dashboard-card">
                <Card.Body>
                  <Card.Title>Document Details</Card.Title>
                  <Card.Text>View detailed information about documents</Card.Text>
                  <Button variant="info" onClick={onDetailsDocument} className="dashboard-button">
                    <span className="me-2">🔍</span> Document Details
                  </Button>
                </Card.Body>
              </Card>
            </Col>
            
            <Col md={4} className="mb-4">
              <Card className="dashboard-card">
                <Card.Body>
                  <Card.Title>All Documents</Card.Title>
                  <Card.Text>View all documents in the system</Card.Text>
                  <Button variant="secondary" onClick={onGetAllDocuments} className="dashboard-button">
                    <span className="me-2">📋</span> Get All Documents
                  </Button>
                </Card.Body>
              </Card>
            </Col>
            
            <Col md={4} className="mb-4">
              <Card className="dashboard-card">
                <Card.Body>
                  <Card.Title>Search Documents</Card.Title>
                  <Card.Text>Find documents by name</Card.Text>
                  <Button variant="warning" onClick={onGetByName} className="dashboard-button">
                    <span className="me-2">🔎</span> Get By Name
                  </Button>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Container>
      </div>
    </div>
  );
};

export default Dashboard;